using Hexa_Hub.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Hexa_Hub.Repository
{
    public class AssetService : IAsset
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _environment;

        public AssetService(DataContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<List<Asset>> GetAllAssets()
        {
            return await _context.Assets
                                 .Include(a => a.Category)
                                 .Include(a => a.SubCategories)
                                 .Include(a => a.AssetRequests)
                                 .Include(a => a.ServiceRequests)
                                 .Include(a => a.MaintenanceLogs)
                                 .Include(a => a.Audits)
                                 .Include(a => a.ReturnRequests)
                                 .Include(a => a.AssetAllocations)
                                 .ToListAsync();
        }

        public async Task<Asset?> GetAssetById(int id)
        {
            return await _context.Assets
                                 .Include(a => a.Category)
                                 .Include(a => a.SubCategories)
                                 .Include(a => a.AssetRequests)
                                 .Include(a => a.ServiceRequests)
                                 .Include(a => a.MaintenanceLogs)
                                 .Include(a => a.Audits)
                                 .Include(a => a.ReturnRequests)
                                 .Include(a => a.AssetAllocations)
                                 .FirstOrDefaultAsync(a => a.AssetId == id);
        }

        public async Task AddAsset(Asset asset)
        {
            _context.Assets.Add(asset);
        }

        public async Task<Asset> UpdateAsset(Asset asset)
        {
            _context.Assets.Update(asset);
            return asset;
        }

        public async Task DeleteAsset(int id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset == null)
            {
            throw new Exception("Asset not Found");
            }

            _context.Assets.Remove(asset);

        }
        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<string?> UploadAssetImageAsync(int userId, IFormFile file)
        {
            var assetData = await _context.Assets.FindAsync(userId);
            if(assetData == null) 
            { 
                return null; 
            }
            string imagePath = Path.Combine(_environment.ContentRootPath, "AssetImages");
            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }
            if (assetData.AssetImage == null && file == null)
            {
                string defaultImagePath = GetDefaultAssetImagePath();
                assetData.AssetImage = await GetImageBytesAsync(defaultImagePath);
            }
            else if(file != null)
            {
                string fileName = $"{userId}_{Path.GetFileName(file.FileName)}";
                string fullPath = Path.Combine(imagePath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                assetData.AssetImage = await File.ReadAllBytesAsync(fullPath);
            }
            await _context.SaveChangesAsync();
            return file?.FileName ?? "AssetDefault.jpg";

        }

        public string GetDefaultAssetImagePath()
        {
            return Path.Combine(_environment.ContentRootPath, "Images", "AssetDefault.jpg");
        }
        private async Task<byte[]> GetImageBytesAsync(string path)
        {
            return await File.ReadAllBytesAsync(path);
        }
    }

}
