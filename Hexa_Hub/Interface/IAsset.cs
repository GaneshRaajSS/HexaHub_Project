namespace Hexa_Hub.Interface
{
    public interface IAsset
    {
        Task<List<Asset>> GetAllAssets();
        Task<Asset?> GetAssetById(int id);
        Task AddAsset(Asset asset);
        Task<Asset> UpdateAsset(Asset asset);
        Task DeleteAsset(int id);
        Task Save();
        Task<string?> UploadAssetImageAsync(int assetId, IFormFile file);
        string GetDefaultAssetImagePath();
    }

}
