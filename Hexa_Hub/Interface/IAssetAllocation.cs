namespace Hexa_Hub.Interface
{
    public interface IAssetAllocation
    {
        Task<List<AssetAllocation>> GetAllAllocations();
        Task<AssetAllocation?> GetAllocationById(int id);
        Task AddAllocation(AssetAllocation allocation);
        Task<AssetAllocation> UpdateAllocation(AssetAllocation allocation);
        Task DeleteAllocation(int id);
        Task Save();
        Task<List<AssetAllocation>> GetAllocationListById(int userId);
    }

}
