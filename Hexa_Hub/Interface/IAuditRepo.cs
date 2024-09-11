namespace Hexa_Hub.Interface
{
    public interface IAuditRepo
    {
        Task<List<Audit>> GetAllAudits();
        Task<Audit?> GetAuditById(int id);
        Task AddAuditReq(Audit audit);
        Task<Audit> UpdateAudit(Audit audit);
        Task DeleteAuditReq(int id);
        Task Save();
        Task<List<Audit>> GetAuditsByUserId(int userId);
    }
}
