namespace Hexa_Hub.Interface
{
    public interface IAssetRequest
    {
        Task<List<AssetRequest>> GetAllAssetRequests();
        Task<AssetRequest?> GetAssetRequestById(int id);
        Task AddAssetRequest(AssetRequest assetRequest);
        Task<AssetRequest> UpdateAssetRequest(AssetRequest assetRequest);
        Task DeleteAssetRequest(int id);
        Task Save();
        Task<List<AssetRequest>> GetAssetRequestsByUserId(int userId);
    }

}
