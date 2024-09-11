using Hexa_Hub.Interface;
using Microsoft.EntityFrameworkCore;

namespace Hexa_Hub.Repository
{
    public class MaintenanceLogRepo : IMaintenanceLogRepo
    {
        private readonly DataContext _context;

        public MaintenanceLogRepo(DataContext context)
        {
            _context = context;
        }

        public async Task<List<MaintenanceLog>> GetAllMaintenanceLog()
        {
            return await _context.MaintenanceLogs
                .Include(ml => ml.Asset)
                .Include(ml => ml.User)
                .ToListAsync();
        }

        public async Task<MaintenanceLog?> GetMaintenanceLogById(int id)
        {
            return await _context.MaintenanceLogs
                .Include(ml => ml.Asset)
                .Include(ml => ml.User)
                .FirstOrDefaultAsync(ml=>ml.MaintenanceId==id);
        }

        public async Task AddMaintenanceLog(MaintenanceLog maintenanceLog)
        {
            _context.MaintenanceLogs.AddAsync(maintenanceLog);
        }

        public async Task DeleteMaintenanceLog(int id)
        {
            var log = await _context.MaintenanceLogs.FindAsync(id);
            if (log == null)
            {
                throw new Exception("Log Not Found");
            }
            _context.MaintenanceLogs.Remove(log);

        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public Task<MaintenanceLog> UpdateMaintenanceLog(MaintenanceLog maintenanceLog)
        {
            _context.MaintenanceLogs.Update(maintenanceLog);
            return Task.FromResult(maintenanceLog);
        }


        public async Task<List<MaintenanceLog>> GetMaintenanceLogByUserId(int userId)
        {
            return await _context.MaintenanceLogs
                .Where(ml => ml.UserId == userId)
                .Include(ml => ml.Asset)
                .Include(ml => ml.User)
                .ToListAsync();
        }
    }
}
