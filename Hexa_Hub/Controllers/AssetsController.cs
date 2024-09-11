using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hexa_Hub.Interface;
using Hexa_Hub.Repository;
using System.Security.Claims;

namespace Hexa_Hub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IAsset _asset;

        public AssetsController(DataContext context, IAsset asset)

        {
            _context = context;
            _asset = asset;
        }

        // GET: api/Assets
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Asset>>> GetAssets()
        {
            return await _asset.GetAllAssets();
        }

        // PUT: api/Assets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutAsset(int id, Asset asset)
        {
            if (id != asset.AssetId)
            {
                return BadRequest();
            }
            _asset.UpdateAsset(asset);
            try
            {
                await _asset.Save();
                _asset.UpdateAsset(asset);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssetExists(id))
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

        // POST: api/Assets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Asset>> PostAsset(Asset asset)
        {
            _asset.AddAsset(asset);
            await _asset.Save();

            return CreatedAtAction("GetAssets", new { id = asset.AssetId }, asset);
        }

        // DELETE: api/Assets/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAsset(int id)
        {
            try
            {
                await _asset.DeleteAsset(id);
                await _asset.Save();
                return NoContent();
            }
            catch (Exception)
            {
                if (id == null)
                    return NotFound();
                return BadRequest();
            }
        }

        private bool AssetExists(int id)
        {
            return _context.Assets.Any(e => e.AssetId == id);
        }

        [HttpPut("{assetId}/upload")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadAssetImage(int assetId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            var supportedFiles = new[] { "image/jpeg", "image/png" };
            if (!supportedFiles.Contains(file.ContentType))
            {
                return BadRequest("Only JPEG or PNG format are allowed");
            }
            var fileName = await _asset.UploadAssetImageAsync(assetId, file);
            if (fileName == null)
            {
                return NotFound();
            }

            return Ok(new { FileName = fileName });
        }

        //image
        [HttpGet("{assetId}/assetImage")]
        [Authorize]
        public async Task<IActionResult> GetAssetImage(int assetId)
        {
            var asset = await _asset.GetAssetById(assetId);
            if (asset == null || asset.AssetImage == null)
            {
                var defualtImagePath = _asset.GetDefaultAssetImagePath();
                return PhysicalFile(defualtImagePath, "image/jpeg");
            }
            using (var memoryStream = new MemoryStream(asset.AssetImage))
            {
                var fileExtensions = Path.GetExtension("AssetDefault.jpg").ToLowerInvariant();
                var contentType = fileExtensions switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    _ => "application/octet-stream"
                };
                return File(memoryStream.ToArray(), contentType);
            }
        }
    }
}
