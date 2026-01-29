using AccountDAL.Eentiti;
using DashBoard.Errors;
using DashBoard.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DashBoard.Controllers
{
   
    public class AccountDashController : Controller
    {
        private readonly UserManager<DashAppUser> _userManager;
        private readonly SignInManager<DashAppUser> _signInManager;

        public AccountDashController( UserManager<DashAppUser> userManager ,SignInManager<DashAppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        //public IActionResult Index()
        //{
        //    return View();
        //}
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            var user = await _userManager.FindByEmailAsync(loginViewModel.Email);
            if (user == null) return Unauthorized(new ApiResponse(401));
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginViewModel.Password, false);
            if (!result.Succeeded) return Unauthorized(new ApiResponse(401));

            var repo = await _signInManager.PasswordSignInAsync(user, loginViewModel.Password, loginViewModel.RememberMe, false);
            if (repo.Succeeded)
            {
                //var Token = await _tokenService.CreateToken(user, _userManager);
                return RedirectToAction("HomePage", "CozaMaster");
            }
            return Unauthorized(new ApiResponse(401));
            
        }
        [HttpGet]
        [Authorize]
        public IActionResult Register()
        {
            return View(); 
        }  


        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (CheckEmailExistAsync(registerViewModel.Email).Result.Value)
            {
                return new BadRequestObjectResult(new ApiValidationErrorResponse() { Errors = new[] { "Email address is in use" } });
            }
            //registerDto.ImegeName = DocumentSettings.UploudFile(registerDto.Imege, "image");

            var user = new DashAppUser()
            {
                FristNames = registerViewModel.FristNames,
                Email = registerViewModel.Email,
                UserName = registerViewModel.Email.Split('@')[0]


            };

            var result = await _userManager.CreateAsync(user, registerViewModel.Password);

            if (!result.Succeeded) return BadRequest(new ApiResponse(400));

            return RedirectToAction("Login", "AccountDash");
        }
        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmailExistAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }
    }
}
