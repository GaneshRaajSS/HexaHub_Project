namespace Hexa_Hub.Interface
{
    public interface IReturnReqRepo
    {
        Task<List<ReturnRequest>> GetAllReturnRequest();
        Task<ReturnRequest?> GetReturnRequestById(int id);
        Task AddReturnRequest(ReturnRequest returnRequest);
        public void UpdateReturnRequest(ReturnRequest returnRequest);
        Task DeleteReturnRequest(int id);
        Task Save();
        Task<List<ReturnRequest>> GetReturnRequestsByUserId(int userId);

        Task<bool> UserHasAsset(int id);
    }
}
