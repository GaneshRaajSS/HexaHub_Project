using Hexa_Hub.Interface;
using Microsoft.EntityFrameworkCore;

namespace Hexa_Hub.Repository
{
    public class ServiceRequestImpl : IServiceRequest
    {
        private readonly DataContext _context;

        public ServiceRequestImpl(DataContext context)
        {
            _context = context;
        }

        public async Task<List<ServiceRequest>> GetAllServiceRequests()
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Asset)
                .Include(sr => sr.User)
                .ToListAsync();
        }

        public async Task<ServiceRequest?> GetServiceRequestById(int id)
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Asset)
                .Include(sr => sr.User)
                .FirstOrDefaultAsync(u => u.ServiceId == id);
        }

        public async Task AddServiceRequest(ServiceRequest serviceRequest)
        {
            var assetExists = await _context.Assets.AnyAsync(a => a.AssetId == serviceRequest.AssetId);
            if (!assetExists)
            {
                throw new ArgumentException("Invalid AssetId. The asset does not exist.");
            }

            _context.ServiceRequests.Add(serviceRequest);
        }

        public Task<ServiceRequest> UpdateServiceRequest(ServiceRequest existingRequest)
        {
            _context.ServiceRequests.Update(existingRequest);
            return Task.FromResult(existingRequest);
        }

        public async Task DeleteServiceRequest(int id)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);
            if (serviceRequest == null)
            {
                throw new Exception("Service Request not Found");
            }
            _context.ServiceRequests.Remove(serviceRequest);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<ServiceRequest>> GetServiceRequestsByUserId(int userId)
        {
            return await _context.ServiceRequests
                .Where(sr => sr.UserId == userId)
                .Include(sr => sr.Asset)
                .Include(sr => sr.User)
                .ToListAsync();
        }


    }

}
