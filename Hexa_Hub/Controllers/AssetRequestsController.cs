using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Hexa_Hub.Interface;
using Hexa_Hub.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Hexa_Hub.Models.MultiValues;

namespace Hexa_Hub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetRequestsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IAssetRequest _assetRequest;
        private readonly IAssetAllocation _assetAlloc;
        private readonly IAsset _asset;
        public AssetRequestsController(DataContext context, IAssetRequest assetRequest, IAssetAllocation assetAlloc, IAsset asset)
        {
            _context = context;
            _assetRequest = assetRequest;
            _assetAlloc = assetAlloc;
            _asset = asset;
        }

        // GET: api/AssetRequests
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AssetRequest>>> GetAssetRequests()
        {
            //User can see his own details whereas Admin can see all users details

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            if (userRole == "Admin")
            {
                return await _assetRequest.GetAllAssetRequests();
            }
            else
            {
                var req = await _assetRequest.GetAssetRequestsByUserId(userId);
                if (req == null)
                {
                    return NotFound();
                }
                return Ok(req);
            }
        }



        // PUT: api/AssetRequests/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutAssetRequest(int id, AssetRequest assetRequest)
        {
            //when an asset is being set to allocated by admin it automaticaly sets data to AssetAllocation Table
            if (id != assetRequest.AssetReqId)
            {
                return BadRequest();
            }
            _assetRequest.UpdateAssetRequest(assetRequest);
            if(assetRequest.Request_Status == RequestStatus.Allocated) 
            {
                var exisitingAllocId = await _context.AssetAllocations
                    .FirstOrDefaultAsync(aa => aa.AssetReqId == assetRequest.AssetReqId);
                if (exisitingAllocId == null)
                {
                    var assetAllocation = new AssetAllocation
                    {
                        AssetId = assetRequest.AssetId,
                        UserId = assetRequest.UserId,
                        AssetReqId = assetRequest.AssetReqId,
                        AllocatedDate = DateTime.Now
                    };
                    await _assetAlloc.AddAllocation(assetAllocation);

                    var asset = await _context.Assets.FindAsync(assetRequest.AssetId);
                    if (asset != null)
                    {
                        asset.Asset_Status = AssetStatus.Allocated;
                        _asset.UpdateAsset(asset);
                    }
                }
            }
            try
            {
                _assetAlloc.Save();
                _asset.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssetRequestExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/AssetRequests
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<ActionResult<AssetRequest>> PostAssetRequest(AssetRequest assetRequest)
        {
            //logged user can only create an post req for himself
            var loggedInUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            assetRequest.UserId = loggedInUserId;
            _assetRequest.AddAssetRequest(assetRequest);
            await _assetRequest.Save();


            return CreatedAtAction("GetAssetRequests", new { id = assetRequest.AssetReqId }, assetRequest);
        }

        // DELETE: api/AssetRequests/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> DeleteAssetRequest(int id)
        {
            try
            {
                var loggedInUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var assetRequest = await _assetRequest.GetAssetRequestById(id);
                if (assetRequest == null)
                {
                    return NotFound();
                }
                if (assetRequest.UserId != loggedInUserId)
                {
                    return Forbid();
                }
                if(assetRequest.Request_Status == RequestStatus.Allocated)
                {
                    return Forbid();
                }
                await _assetRequest.DeleteAssetRequest(id);
                await _assetRequest.Save();
                return NoContent();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        private bool AssetRequestExists(int id)
        {
            return _context.AssetRequests.Any(e => e.AssetReqId == id);
        }
    }
}
