namespace Hexa_Hub.Interface
{
    public interface IMaintenanceLogRepo
    {
        Task<List<MaintenanceLog>> GetAllMaintenanceLog();
        Task<MaintenanceLog?> GetMaintenanceLogById(int id);
        Task AddMaintenanceLog(MaintenanceLog maintenanceLog);
        Task<MaintenanceLog> UpdateMaintenanceLog(MaintenanceLog maintenanceLog);
        Task DeleteMaintenanceLog(int id);
        Task Save();

        Task<List<MaintenanceLog>> GetMaintenanceLogByUserId(int userId);
    }
}
