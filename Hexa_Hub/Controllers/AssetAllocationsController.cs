using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Hexa_Hub.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hexa_Hub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetAllocationsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IAssetAllocation _assetallocation;

        public AssetAllocationsController(DataContext context, IAssetAllocation assetAllocation)
        {
            _context = context;
            _assetallocation = assetAllocation;
        }

        // GET: api/AssetAllocations
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AssetAllocation>>> GetAssetAllocations()
        {
            
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            if(userRole == "Admin")
            {
                return await _assetallocation.GetAllAllocations();
            }
            else
            {
                var userRequests = await _assetallocation.GetAllocationListById(userId);
                if (userRequests == null || !userRequests.Any()) {
                    return NotFound();
                }
                return Ok(userRequests);
            }
        }

     
        // DELETE: api/AssetAllocations/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAssetAllocation(int id)
        {

            try
            {
                await _assetallocation.DeleteAllocation(id);
                await _assetallocation.Save();
                return NoContent();
            }
            catch (Exception)
            {
                if (id == null)
                    return NotFound();
                return BadRequest();
            }
        }

        private bool AssetAllocationExists(int id)
        {
            return _context.AssetAllocations.Any(e => e.AllocationId == id);
        }
    }
}
