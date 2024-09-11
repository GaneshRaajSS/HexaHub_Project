using Hexa_Hub.Interface;
using Microsoft.EntityFrameworkCore;

namespace Hexa_Hub.Repository
{
    public class UserProfileRepo : IUserProfileRepo
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _environment;
        public UserProfileRepo(DataContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public async Task AddProfiles(UserProfile userProfile)
        {
            if (userProfile.ProfileImage == null)
            {
                string defaultImagePath = GetDefaultImagePath();
                userProfile.ProfileImage = await GetImageBytesAsync(defaultImagePath);
            }
            await _context.AddAsync(userProfile);
        }

        public async Task DeleteProfiles(int id)
        {
            var profile = await _context.UserProfiles.FindAsync(id);
            if (profile == null)
            {
                throw new Exception("Profile Not Found");
            }
            _context.UserProfiles.Update(profile);
        }

        //public async Task<List<UserProfile>> GetAllProfiles()
        //{
        //    return await _context.UserProfiles
        //        .Include(up=>up.User)
        //        .ToListAsync();
        //}

        public async Task<UserProfile?> GetProfilesById(int id)
        {
            return await _context.UserProfiles
                .Include(up=>up.User)
                .FirstOrDefaultAsync(up=>up.UserId == id);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        public Task<UserProfile> UpdateProfiles(UserProfile userProfile)
        {
            _context.UserProfiles.Update(userProfile);
            return Task.FromResult(userProfile);
        }

        public async Task<string?> UploadProfileImageAsync(int userId, IFormFile file)
        {
            var userProfile = await _context.UserProfiles.FindAsync(userId);
            if (userProfile == null) 
            { 
                return null; 
            }

            string imagePath = Path.Combine(_environment.ContentRootPath, "Images");

            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }
            if (userProfile.ProfileImage == null && file == null)
            {
                string defaultImagePath = GetDefaultImagePath();
                userProfile.ProfileImage = await GetImageBytesAsync(defaultImagePath);
            }
            else if (file != null)
            {
                string fileName = $"{userId}_{Path.GetFileName(file.FileName)}";
                string fullPath = Path.Combine(imagePath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                userProfile.ProfileImage = await File.ReadAllBytesAsync(fullPath);
            }

            await _context.SaveChangesAsync();
            return file?.FileName ?? "profile-img.jpg";
        }

        public string GetDefaultImagePath()
        {
            return Path.Combine(_environment.ContentRootPath, "Images", "profile-img.jpg");
        }
        private async Task<byte[]> GetImageBytesAsync(string path)
        {
            return await File.ReadAllBytesAsync(path);
        }
    }
}
