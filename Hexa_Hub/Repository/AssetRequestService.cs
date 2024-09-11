using Hexa_Hub.Interface;
using Microsoft.EntityFrameworkCore;

namespace Hexa_Hub.Repository
{
    public class AssetRequestService : IAssetRequest
    {
        private readonly DataContext _context;

        public AssetRequestService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<AssetRequest>> GetAllAssetRequests()
        {
            return await _context.AssetRequests
                .Include(ar=>ar.Asset)
                .Include(ar=>ar.User)
                .ToListAsync();
        }

        public async Task<List<AssetRequest>> GetAssetRequestsByUserId(int userId)
        {
            return await _context.AssetRequests
                .Where(sr => sr.UserId == userId)
                .Include(sr => sr.Asset)
                .Include(sr => sr.User)
                .ToListAsync();
        }

        public async Task<AssetRequest?> GetAssetRequestById(int id)
        {
            return await _context.AssetRequests
                .Include(ar => ar.Asset)
                .Include(ar => ar.User)
                .FirstOrDefaultAsync(u => u.AssetReqId == id);
        }

        public async Task AddAssetRequest(AssetRequest assetRequest)
        {
            _context.AssetRequests.Add(assetRequest);
        }

        public Task<AssetRequest> UpdateAssetRequest(AssetRequest assetRequest)
        {
            _context.AssetRequests.Update(assetRequest);
            return Task.FromResult(assetRequest);

        }

        public async Task DeleteAssetRequest(int id)
        {
            var assetRequest = await _context.AssetRequests.FindAsync(id);
            if (assetRequest == null)
            {
                throw new Exception("Request not found");
            }

            _context.AssetRequests.Remove(assetRequest);

        }
        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }

}
