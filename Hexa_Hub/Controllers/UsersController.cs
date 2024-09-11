using Hexa_Hub.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Hexa_Hub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepo _userRepo;
        private readonly IUserProfileRepo _userProfileRepo;
        private readonly DataContext _context;

        public UsersController(DataContext context, IUserRepo userRepo, IUserProfileRepo userProfileRepo)
        {
            _context = context;
            _userRepo = userRepo;
            _userProfileRepo = userProfileRepo;
        }

        // GET: api/Users
        [HttpGet]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _userRepo.GetAllUser();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _userRepo.GetUserById(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            if (id != user.UserId)
            {
                return BadRequest();
            }
            if (userRole != "Admin")
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (id != userId)
                {
                    return Forbid();
                }
            }

            _userRepo.UpdateUser(user);

            var userProfile = await _userProfileRepo.GetProfilesById(id);
            if (userProfile != null)
            {
                userProfile.UserName = user.UserName;
                userProfile.UserMail = user.UserMail;
                userProfile.Gender = user.Gender;
                userProfile.Dept = user.Dept;
                userProfile.Designation = user.Designation;
                userProfile.PhoneNumber = user.PhoneNumber;
                userProfile.Address = user.Address;

                _userProfileRepo.UpdateProfiles(userProfile);
            }

            try
            {
                await _userRepo.Save();
                await _userProfileRepo.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _userRepo.AddUser(user);
            await _userRepo.Save();

            var userProfile = new UserProfile
            {
                UserId = user.UserId,
                UserName = user.UserName,
                UserMail = user.UserMail,
                Gender = user.Gender,
                Dept = user.Dept,
                Designation = user.Designation,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address
            };
             await _userProfileRepo.AddProfiles(userProfile);
            await _userProfileRepo.Save();

            return CreatedAtAction("GetUser", new { id = user.UserId }, user);
        }

        // DELETE: api/Users/5

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userRepo.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                await _userRepo.DeleteUser(id);
                await _userRepo.Save();

                var userProfile = await _userProfileRepo.GetProfilesById(id);
                if (userProfile != null)
                {
                    await _userProfileRepo.DeleteProfiles(id);
                    await _userProfileRepo.Save();
                }
            }
            catch (Exception)
            {
                return NotFound("Error deleting the user");
            }

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
