using Hexa_Hub.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static Hexa_Hub.Models.MultiValues;

namespace Hexa_Hub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfilesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IUserProfileRepo _userProfile;

        public UserProfilesController(DataContext context, IUserProfileRepo userProfileRepo, IWebHostEnvironment environment)
        {
            _context = context;
            _userProfile = userProfileRepo;
            _environment = environment;
        }

        //// GET: api/UserProfiles
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<UserProfile>>> GetUserProfiles()
        //{
        //    //return await _context.UserProfiles.ToListAsync();
        //    return await _userProfile.GetAllProfiles();
        //}

        // GET: api/UserProfiles/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserProfile>> GetUserProfile(int id)
        {
            var userProfile = await _userProfile.GetProfilesById(id);

            if (userProfile == null)
            {
                return NotFound();
            }

            return userProfile;
        }

        // PUT: api/UserProfiles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutUserProfile(int id, UserProfile userProfile, UserType newRole)
        {
            if (id != userProfile.UserId)
            {
                return BadRequest();
            }
            var existingProfile = await _userProfile.GetProfilesById(id);
            if (existingProfile == null)
            {
                return NotFound("UserProfile not found.");
            }

            existingProfile.UserName = userProfile.UserName;
            existingProfile.UserMail = userProfile.UserMail;
            existingProfile.Gender = userProfile.Gender;
            existingProfile.Dept = userProfile.Dept;
            existingProfile.Designation = userProfile.Designation;
            existingProfile.PhoneNumber = userProfile.PhoneNumber;
            existingProfile.Address = userProfile.Address;

            var user = existingProfile.User;
            if (user == null)
            {
                return NotFound("Associated User not found.");
            }

            if (user.User_Type != newRole)
            {
                user.User_Type = newRole;
            }
            
            try
            {
                await _userProfile.UpdateProfiles(existingProfile);
                await _userProfile.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserProfileExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool UserProfileExists(int id)
        {
            return _context.UserProfiles.Any(e => e.UserId == id);
        }

        [HttpPut("{userId}/upload")]
        [Authorize]
        public async Task<IActionResult> UploadProfileImage(int userId, IFormFile file)
        {
            var loggedUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if(loggedUser != userId)
            {
                return Unauthorized("You are not Authorized to Update the Image");
            }
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            var supportedFiles = new[] {  "image/jpeg", "image/png" };
            if (!supportedFiles.Contains(file.ContentType))
            {
                return BadRequest("Only JPEG or PNG format are allowed");
            }
            var fileName = await _userProfile.UploadProfileImageAsync(userId, file);
            if (fileName == null)
            {
                return NotFound();
            }

            return Ok(new { FileName = fileName });
        }
        [Authorize]
        [HttpGet("{userId}/profileImage")]
        public async Task<IActionResult> GetProfileImage(int userId)
        {
            var userProfile = await _userProfile.GetProfilesById(userId);
            if(userProfile == null || userProfile.ProfileImage == null)
            {
                var defualtImagePath = _userProfile.GetDefaultImagePath();
                return PhysicalFile(defualtImagePath, "image/jpeg");
            }
            using (var memoryStream = new MemoryStream(userProfile.ProfileImage))
            {
                var fileExtensions = Path.GetExtension("profile-img.jpg").ToLowerInvariant();
                var contentType = fileExtensions switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    _ => "application/octet-stream"
                };
                return File(memoryStream.ToArray(), contentType);
            }
        }
    }
}
