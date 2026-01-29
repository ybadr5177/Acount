using AccountDAL;
using AccountDAL.config;
using AccountDAL.Eentiti;

using Acount.Dto;
using Acount.Errors;
using Acount.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
 

namespace Acount.Controllers.Account1
{
    [Route("api/[controller]")]
    [ApiController]
    public class Account : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly EmailService _emailService;
        private readonly ITokenService _tokenService;
        private readonly AppIdentityDbContext _context;


        public Account(UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            EmailService emailService,
            ITokenService tokenService,
            AppIdentityDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _tokenService = tokenService;
            _context = context;
        }


        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) return Unauthorized(new ApiResponse(401));
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded) return Unauthorized(new ApiResponse(401));


            return Ok(new UserDto()
            {
                FristName = user.FristNames,
                Email = user.Email,
                Token = await _tokenService.CreateToken(user, _userManager)
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromForm] RegisterDto registerDto)
        {
            if (CheckEmailExistAsync(registerDto.Email).Result.Value)
            {
                return new BadRequestObjectResult(new ApiValidationErrorResponse() { Errors = new[] { "Email address is in use" } });
            }
            if (registerDto.Imege != null)
            {
                registerDto.ProfilePicture = DocumentSettings.UploudFile(registerDto.Imege, "image");
            }
           var addressre = new AccountDAL.Eentiti.Address()
           {
               City= registerDto.City, 
               Street= registerDto.Street,
               Region =registerDto.Region,
               Country=registerDto.Country,
              
           };

            var user = new AppUser()
            {
                FristNames = registerDto.FristNames,
                Email = registerDto.Email,
                UserName = registerDto.Email.Split('@')[0],
                PhoneNumber = registerDto.PhoneNumber,
                Gender= registerDto.Gender,
                ProfilePicture = registerDto.ProfilePicture ?? "mg",
                LastName = registerDto.LastName,
                FullNames = registerDto.FullNames,
                Address = addressre,
                DateOfBirth = registerDto.DateOfBirth ?? default

            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new ApiValidationErrorResponse
                {
                    Errors = new[] { $"User creation failed: {errors}" }
                });
            }

            return Ok(new UserDto()
            {
                FristName = user.FristNames,
                Email = user.Email,
                Token = await _tokenService.CreateToken(user, _userManager)
            });
        }
        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmailExistAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }
        //[HttpPost("ForgetPassword")]
        //public async Task<ActionResult<string>> ForgetPassword(ForgetPasswordDto forgetPasswordDto)
        //{

        //    if (CheckEmailExistAsync(forgetPasswordDto.Email).Result.Value)
        //    {
        //        var user = await _userManager.FindByEmailAsync(forgetPasswordDto.Email);
        //        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        //        var resetUrl = Url.Action("ResetPassword", "Account", new { Email = forgetPasswordDto.Email,Token= token }, Request.Scheme);

        //        string subject = "Reset your Password";
        //        await _emailService.SendEmailAsync(forgetPasswordDto.Email, subject, resetUrl);
        //        return Ok("Check your in Box");
        //    }
        //    return BadRequest(new ApiResponse(401));
        //}
        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordDto forgetPasswordDto)
        {
            if (string.IsNullOrEmpty(forgetPasswordDto.Email))
                return BadRequest(new ApiResponse(400, "Email is required"));

            if (!CheckEmailExistAsync(forgetPasswordDto.Email).Result.Value)
                return BadRequest(new ApiResponse(404, "Email not found"));

            var user = await _userManager.FindByEmailAsync(forgetPasswordDto.Email);

            // توليد OTP
            Random random = new Random();
            int verificationCode = random.Next(1000, 9999);

            // نخزن الكود والإيميل في السيشن
            HttpContext.Session.SetInt32("VerificationCode", verificationCode);
            HttpContext.Session.SetString("ResetEmail", forgetPasswordDto.Email);

            // إرسال الإيميل
            string subject = "Password Reset - Verification Code";
            string message = $"Your verification code is: {verificationCode}";
            await _emailService.SendEmailAsync(forgetPasswordDto.Email, subject, message);

            return Ok(new ApiResponse(200, "Verification code sent to your email."));
        }
        //[HttpPost("SendVerificationCode")]
        //public async Task<IActionResult> SendVerificationCode(string email)
        //{
        //    if (string.IsNullOrEmpty(email))
        //    {
        //        return BadRequest(new ApiResponse(401));
        //    }

        //    Random random = new Random();
        //    int verificationCode = random.Next(1000, 9999);


        //    HttpContext.Session.SetInt32("VerificationCode", verificationCode);


        //    string subject = "Your verification code";
        //    string message = $"Your verification code is: {verificationCode}";

        //    await _emailService.SendEmailAsync(email, subject, message);

        //    return Ok("A verification code has been sent to your email.");
        //}
        [HttpPost("SendVerificationCode")]
        public async Task<IActionResult> SendVerificationCode(string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest(new ApiResponse(400, "Email is required"));

            if (!CheckEmailExistAsync(email).Result.Value)
                return BadRequest(new ApiResponse(404, "Email not found"));

            Random random = new Random();
            int verificationCode = random.Next(1000, 9999);

            HttpContext.Session.SetInt32("VerificationCode", verificationCode);
            HttpContext.Session.SetString("ResetEmail", email);

            string subject = "Password Reset - Verification Code (Resend)";
            string message = $"Your new verification code is: {verificationCode}";
            await _emailService.SendEmailAsync(email, subject, message);

            return Ok(new ApiResponse(200, "A new verification code has been sent."));
        }
        //[HttpPost("VerifyCode")]
        //public IActionResult VerifyCode(int code)
        //{
        //    int? storedCode = HttpContext.Session.GetInt32("VerificationCode");

        //    if (storedCode == null)
        //    {
        //        return BadRequest("Verification code not found. Please order again.");
        //    }

        //    if (storedCode == code)
        //    {
        //        HttpContext.Session.Remove("VerificationCode");
        //        return Ok("Verified successfully!");
        //    }

        //    return BadRequest("The verification code is invalid.");
        //}
        [HttpPost("VerifyCode")]
        public async Task<IActionResult> VerifyCode(OTPDto code)
        {
            int? storedCode = HttpContext.Session.GetInt32("VerificationCode");
            string email = HttpContext.Session.GetString("ResetEmail");

            if (storedCode == null || string.IsNullOrEmpty(email))
                return BadRequest(new ApiResponse(400, "No verification request found. Please try again."));

            if (storedCode != code.OtpCode)
                return BadRequest(new ApiResponse(401, "Invalid verification code."));

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return BadRequest(new ApiResponse(404, "User not found."));

            // لو الكود صح، نولد Token للـ Reset
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // ممكن ترجعه للـ Frontend يخزنه لحد ما يعمل ResetPassword
            return Ok(new
            {
                Message = "Verification successful!",
                ResetToken = token,
                Email = email
            });
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto Reset)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(Reset.Email);
            if (user == null)
                return BadRequest(new ApiResponse(400, "Invalid data"));

            var result = await _userManager.ResetPasswordAsync(user, Reset.Token, Reset.NewPassword);

            if (!result.Succeeded)
                return NotFound(new ApiResponse(404, "User not found"));

            return Ok(new ApiResponse(200, "Password has been reset successfully."));
        }
        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            var email = User.FindFirstValue(ClaimTypes.Email); // من الـ Token
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) return NotFound(new ApiResponse(404));

            var address = await _context.Address.FirstOrDefaultAsync(a => a.Id == user.AddressId);
            return new UserDto()
            {
                FristName = user.FristNames,
                LastName = user.LastName,
                FullNames = user.FullNames,
                Email = user.Email,
                ProfilePicture = user.ProfilePicture,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName,
                Address= user.Address,
                
                Token = await _tokenService.CreateToken(user, _userManager)
            };
        }
        //[Authorize]
        [HttpPut("profile")]
        public async Task<ActionResult<UserDto>> Profile([FromForm] UpdateProfileDto updateDto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) return NotFound(new ApiResponse(404));

            user.FristNames = updateDto.FristNames ?? user.FristNames;
            user.LastName = updateDto.LastName ?? user.LastName;
            user.FullNames = updateDto.FullNames ?? user.FullNames;
            user.UserName = updateDto.UserName ?? user.UserName;
            user.PhoneNumber = updateDto.PhoneNumber ?? user.PhoneNumber;
            user.DateOfBirth = updateDto.DateOfBirth ?? user.DateOfBirth;
            user.Gender = updateDto.Gender ?? user.Gender;

            if (updateDto.ProfileFilePicture != null)
            {
                user.ProfilePicture = DocumentSettings.UploudFile(updateDto.ProfileFilePicture, "image");
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded) return BadRequest(new ApiResponse(400));

            return new UserDto()
            {
                FristName = user.FristNames,
                Email = user.Email,
                Token = await _tokenService.CreateToken(user, _userManager)
            };
        }

        // GET: جميع عناوين المستخدم
        [HttpGet("GetAddresses")]
        public async Task<ActionResult<IEnumerable<ShippingAddressDto>>> GetAddresses()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound(new ApiResponse(404, "User not found"));

           

            var addresses = await _context.ShippingAddress
                                          .Where(a => a.AppUserId == user.Id)
                                          .ToListAsync();

            var result = addresses.Select(a => new ShippingAddressDto
            {
                Id = a.Id,
                Street = a.Street,
                City = a.City,
                Country = a.Country,
                FirstName = a.FirstName,
                LastName = a.LastName,
                IsDefault = a.IsDefault,
                 PhoneNumber = a.PhoneNumber
            }).ToList();

            return Ok(result);
        }

        // POST: إضافة عنوان جديد
        [HttpPost("AddAddresses")]
        public async Task<ActionResult<ShippingAddressDto>> AddAddress(ShippingAddressDto addressdto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound(new ApiResponse(404, "User not found"));
           

            var Adderss = new ShippingAddress()
            {
                AppUserId = user.Id,
                Street = addressdto.Street,
                City = addressdto.City,
                Country = addressdto.Country,
                FirstName = addressdto.FirstName,
                LastName = addressdto.LastName,
                IsDefault = addressdto.IsDefault,
                PhoneNumber = addressdto.PhoneNumber

            };
            if (addressdto.IsDefault)
            {
                var userAddresses = _context.ShippingAddress
                    .Where(a => a.AppUserId == user.Id && a.IsDefault)
                    .ToList();
                foreach (var addr in userAddresses)
                    addr.IsDefault = false;

                Adderss.IsDefault = true;
            }
            else
            {
                Adderss.IsDefault = false;
            }
            await _context.ShippingAddress.AddAsync(Adderss);
            await _context.SaveChangesAsync();

            return Ok(new ShippingAddressDto
            {
                Id = Adderss.Id,
                Street = Adderss.Street,
                City = Adderss.City,
                Country = Adderss.Country,
                FirstName = Adderss.FirstName,
                LastName = Adderss.LastName,
                IsDefault = Adderss.IsDefault,
                PhoneNumber = Adderss.PhoneNumber
            });
        }

        // PUT: تعديل عنوان موجود
        [HttpPut("EditAddress")]
        public async Task<ActionResult<ShippingAddressDto>> EditAddress(UpdatedShippingAddressDto updatedAddress)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound(new ApiResponse(404, "User not found"));

            var address = await _context.ShippingAddress
                                        .FirstOrDefaultAsync(a => a.Id == updatedAddress.Id && a.AppUserId == user.Id);

            if (address == null) return NotFound(new ApiResponse(404, "Address not found"));

            bool isChanged = false;

            // تحديث الحقول بشرط انها مش null ومش نفس القيمة القديمة
            if (!string.IsNullOrEmpty(updatedAddress.Street) && updatedAddress.Street != address.Street)
            {
                address.Street = updatedAddress.Street;
                isChanged = true;
            }

            if (!string.IsNullOrEmpty(updatedAddress.City) && updatedAddress.City != address.City)
            {
                address.City = updatedAddress.City;
                isChanged = true;
            }

            if (!string.IsNullOrEmpty(updatedAddress.Country) && updatedAddress.Country != address.Country)
            {
                address.Country = updatedAddress.Country;
                isChanged = true;
            }

            if (!string.IsNullOrEmpty(updatedAddress.FirstName) && updatedAddress.FirstName != address.FirstName)
            {
                address.FirstName = updatedAddress.FirstName;
                isChanged = true;
            }

            if (!string.IsNullOrEmpty(updatedAddress.LastName) && updatedAddress.LastName != address.LastName)
            {
                address.LastName = updatedAddress.LastName;
                isChanged = true;
            }

            if (!string.IsNullOrEmpty(updatedAddress.PhoneNumber) && updatedAddress.PhoneNumber != address.PhoneNumber)
            {
                address.PhoneNumber = updatedAddress.PhoneNumber;
                isChanged = true;
            }

            // التعامل مع IsDefault
            if (updatedAddress.IsDefault && !address.IsDefault)
            {
                var userAddresses = _context.ShippingAddress
                    .Where(a => a.AppUserId == user.Id && a.IsDefault)
                    .ToList();

                foreach (var addr in userAddresses)
                    addr.IsDefault = false;

                address.IsDefault = true;
                isChanged = true;
            }

            // لو مافيش أي تغيير
            if (!isChanged)
                return BadRequest("لم يتم تعديل أي عنصر");

            await _context.SaveChangesAsync();

            return Ok(new ShippingAddressDto
            {
                Id = address.Id,
                Street = address.Street,
                City = address.City,
                Country = address.Country,
                FirstName = address.FirstName,
                LastName = address.LastName,
                PhoneNumber = address.PhoneNumber,
                IsDefault = address.IsDefault
            });
        }

        // DELETE: حذف عنوان
        [HttpDelete("DeleteAddress/{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email)) return Unauthorized();

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return NotFound(new ApiResponse(404, "User not found"));

            var address = await _context.ShippingAddress
                                        .FirstOrDefaultAsync(a => a.Id == id && a.AppUserId == user.Id);

            if (address == null) return NotFound(new ApiResponse(404, "Address not found"));

            _context.ShippingAddress.Remove(address);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Address deleted successfully" });
        }
    }

}

