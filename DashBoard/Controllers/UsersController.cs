using AccountDAL.Eentiti;
using DashBoard.ViewModel;

using MailKit.Search;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DashBoard.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public UsersController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index( string? search)
        {
            ViewData["Search"] = search ?? "";
            var usersQuery = _userManager.Users.AsQueryable();

            
            if (!string.IsNullOrEmpty(search))
            {
                usersQuery = usersQuery.Where(u => u.Email.Contains(search) || u.UserName.Contains(search));
            }

            var users = await usersQuery.ToListAsync();

            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> TotalCount()
        {
            var count = await _userManager.Users.CountAsync();
            return Json(new { count });
        }

        [HttpGet]
        public async Task<IActionResult> ListByStatus(string? status)
        {
            var now = DateTimeOffset.UtcNow;
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
            {
                var key = status.Trim().ToLowerInvariant();
                if (key == "enabled")
                    query = query.Where(u => u.LockoutEnd == null || u.LockoutEnd <= now);
                else if (key == "disabled")
                    query = query.Where(u => u.LockoutEnd != null && u.LockoutEnd > now);
                // else: all
            }

            var users = await query
                .Select(u => new
                {
                    id = u.Id,
                    email = u.Email,
                    userName = u.UserName,
                    status = (u.LockoutEnd == null || u.LockoutEnd <= now) ? "Enabled" : "Disabled"
                })
                .OrderBy(u => u.email)
                .ToListAsync();

            return Json(users);
        }

        public async Task<IActionResult> Disable(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.LockoutEnd = DateTimeOffset.MaxValue; 
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Enable(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.LockoutEnd = null; 
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(string id) 
        {
            if (id == null) return NotFound();
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var UserView = new UserViewModel()
            {
                Id=user.Id,
                Email= user.Email,
                FristNames= user.FristNames,
                PhoneNumber= user.PhoneNumber,  
                Gender= user.Gender,
                DateOfBirth= user.DateOfBirth,

            };
            return View(UserView);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel UserView)
        {

            
            //UserView.ImegeName = DocumentSettings.UploudFile(UserView.Imege, "image");

            var user = new AppUser() 
            { 
                Id= UserView.Id,
                Email= UserView.Email,
                FristNames= UserView.FristNames,
                PhoneNumber= UserView.PhoneNumber,
                Gender= UserView.Gender,
                //ImegeName= UserView.ImegeName,
                DateOfBirth= UserView.DateOfBirth,
            };
            if (user != null )
            {
                  await  _userManager.UpdateAsync(user);

                var userup = new UserViewModel()
                {
                    Email = user.Email,
                    FristNames = user.FristNames,
                    PhoneNumber = user.PhoneNumber,
                    Gender = user.Gender,
                    ImegeName = user.ProfilePicture,
                    DateOfBirth = user.DateOfBirth,
                };
                return View(userup);
            }

            
            return View(UserView);
        }
    }
}
