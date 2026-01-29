using AccountDAL.Eentiti;
using AccountDAL.Eentiti.CozaStore;
using AutoMapper;
using cozastore.ViewModel;
using cozastore.ViewModel.CozaMaster;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Common;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace cozastore.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMapper _mapper;
        //private readonly SignInManager<AppUser> _signInManager;

        public AccountController(HttpClient httpClient, IHttpClientFactory httpClientFactory, IMapper mapper/*, SignInManager<AppUser> signInManager*/)
        {
            _httpClient = httpClient;

            _httpClientFactory = httpClientFactory;
            _mapper = mapper;
            //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); 
            //_signInManager = signInManager;

        }
        public IActionResult login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            //var response = await _httpClient.PostAsJsonAsync("http://centralwebsite.runasp.net/api/Account/login", model);

            //_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //_httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

            var response = await _httpClient.PostAsJsonAsync("https://centralwebsite.runasp.net/api/Account/login", model);

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"RAW RESPONSE: {response.StatusCode} | {responseContent}");
            //Console.WriteLine($"Response: {response.StatusCode} - {responseContent}");
            if (response.IsSuccessStatusCode)
            {
                var loginResult = await response.Content.ReadFromJsonAsync<loginRequestVIewModel>();

                //var token = await response.Content.ReadAsStringAsync();
                //HttpContext.Session.SetString("AuthToken", token);




                //if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                //{
                //    return Redirect(returnUrl);
                //}
                //else
                //{

                //    return RedirectToAction("Index", "Mastar");
                //}

                Console.WriteLine($"JWT Token: {loginResult.Token}");

                if (loginResult != null && !string.IsNullOrEmpty(loginResult.Token))
                {
                    // تخزين التوكن في الكوكي
                    Response.Cookies.Append("AuthToken", loginResult.Token, new CookieOptions
                    {
                        HttpOnly = true,       // ما يتقراش من الجافاسكريبت
                        Secure = true,         // يشتغل بس على HTTPS
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddHours(1)
                    });

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Mastar");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Token not found in the response.");
                    return View(model);
                }


            }


            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            using var formData = new MultipartFormDataContent();

            // البيانات النصية
            formData.Add(new StringContent(model.FristNames ?? ""), "FristNames");
            formData.Add(new StringContent(model.UserName ?? ""), "UserName");

            formData.Add(new StringContent(model.LastName ?? ""), "LastName");
            formData.Add(new StringContent(model.FullNames ?? ""), "FullNames");
            formData.Add(new StringContent(model.Email ?? ""), "Email");
            formData.Add(new StringContent(model.PhoneNumber ?? ""), "PhoneNumber");
            formData.Add(new StringContent(model.Gender.ToString()), "Gender");
            formData.Add(new StringContent(model.Password ?? ""), "Password");
            formData.Add(new StringContent(model.City ?? ""), "City");
            formData.Add(new StringContent(model.Street ?? ""), "Street");
            formData.Add(new StringContent(model.Region ?? ""), "Region");
            formData.Add(new StringContent(model.Country ?? ""), "Country");
            if (model.DateOfBirth != default)
            {
                formData.Add(new StringContent(model.DateOfBirth.ToString("o")), "DateOfBirth");
            }

            // الملف (الصورة)
            if (model.Imege != null)
            {
                var streamContent = new StreamContent(model.Imege.OpenReadStream());
                formData.Add(streamContent, "Imege", model.Imege.FileName);
            }
            var response = await _httpClient.PostAsync("https://localhost:7133/api/Account/register", formData);

            var responseContent = await response.Content.ReadAsStringAsync();
            //Console.WriteLine($"Response: {response.StatusCode} - {responseContent}");
            if (response.IsSuccessStatusCode)
            {

                var token = await response.Content.ReadAsStringAsync();
                HttpContext.Session.SetString("AuthToken", token);
                return RedirectToAction("Index", "Mastar");
            }


            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            //var token = Request.Cookies["AuthToken"];
            //if (!string.IsNullOrEmpty(token))
            //{
            //    _httpClient.DefaultRequestHeaders.Authorization =
            //        new AuthenticationHeaderValue("Bearer", token);
            //}
            //using var client = new HttpClient(); // HttpClient جديد لكل request
            //client.DefaultRequestHeaders.Authorization =
            //    new AuthenticationHeaderValue("Bearer", token);
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login");

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // نجيب بيانات المستخدم من الـ API
            var response = await client.GetAsync("https://centralwebsite.runasp.net/api/Account/profile");
            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Login");

            var json = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<ProfileViewModel>(json);
            var modelupMap = _mapper.Map<ProfileViewModel, ProfileForMapViewModel>(model);


            //return View(modelupMap);
            return PartialView("_ProfilePopup", modelupMap);
        }

        [HttpGet]
        public async Task<IActionResult> ProfileImageUrl()
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login");

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // نجيب بيانات المستخدم من الـ API
            var response = await client.GetAsync("https://centralwebsite.runasp.net/api/Account/profile");
            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Login");

            var json = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<ProfileViewModel>(json);
            var modelupMap = _mapper.Map<ProfileViewModel, ProfileForMapViewModel>(model);


            //return View(modelupMap);
            return Content(modelupMap.ProfilePicture);
        }
        public async Task<IActionResult> ViewProfile()
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login");

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // نجيب بيانات المستخدم من الـ API
            var response = await client.GetAsync("https://centralwebsite.runasp.net/api/Account/profile");
            if (!response.IsSuccessStatusCode)
                return RedirectToAction("Login");

            var json = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<ProfileViewModel>(json);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login");

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            using var content = new MultipartFormDataContent();

            // نضيف الحقول لو فيها بيانات (مش null أو فارغة)
            if (!string.IsNullOrWhiteSpace(model.FristName))
                content.Add(new StringContent(model.FristName), "FristNames");

            if (!string.IsNullOrWhiteSpace(model.LastName))
                content.Add(new StringContent(model.LastName), "LastName");

            if (!string.IsNullOrWhiteSpace(model.FullNames))
                content.Add(new StringContent(model.FullNames), "FullNames");

            if (!string.IsNullOrWhiteSpace(model.UserName))
                content.Add(new StringContent(model.UserName), "UserName");

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
                content.Add(new StringContent(model.PhoneNumber), "PhoneNumber");


            // لو في صورة
            if (model.ProfileFilePicture != null && model.ProfileFilePicture.Length > 0)
            {
                var streamContent = new StreamContent(model.ProfileFilePicture.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(model.ProfileFilePicture.ContentType);
                content.Add(streamContent, "ProfileFilePicture", model.ProfileFilePicture.FileName);
            }
            //var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("https://centralwebsite.runasp.net/api/Account/Profile", content);

            if (response.IsSuccessStatusCode)
                TempData["Success"] = "Profile updated successfully";

            //return View(model);
            return PartialView("_ProfilePopup", model);
        }
        [HttpGet]
        public IActionResult ForgetPasswordRequest()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel dto)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("https://localhost:7133/api/Account/ForgetPassword", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["Email"] = dto.Email; // نخزن الإيميل مؤقتًا
                return RedirectToAction("VerifyCode", "Account", new { email = dto.Email });
            }

            ViewBag.Error = "Email not found";
            return View(dto);
        }
        //[HttpGet]

        [HttpPost]
        public async Task<IActionResult> SendVerificationCode(string email)
        {
            email ??= TempData["Email"]?.ToString();
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("ForgetPasswordRequest");

            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsync($"https://localhost:7133/api/Account/SendVerificationCode?email={email}", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["Email"] = email;
                TempData["Message"] = "Verification code sent to your email.";
                return RedirectToAction("VerifyCodePage");
            }

            TempData["Error"] = "Failed to send verification code.";
            return View();
        }
        //[HttpPost]
        //public async Task<IActionResult> VerifyCode(int code)
        //{
        //    var client = _httpClientFactory.CreateClient();
        //    var response = await client.PostAsync($"https://localhost:7133/api/Account/VerifyCode?code={code}", null);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        TempData["Verified"] = true;
        //        return RedirectToAction("ResetPasswordPage");
        //    }

        //    TempData["Error"] = "Invalid verification code.";
        //    return View();
        //}
        [HttpGet]
        public IActionResult VerifyCode(string email)
        {
            var data = new OTPViewModel { Email = email };
            return View(data);
        }

        // POST: يبعث الكود للـ API
        [HttpPost]
        public async Task<IActionResult> VerifyCode(OTPViewModel code)
        {
            var email = TempData["Email"]?.ToString();
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("ForgetPassword");

            var client = _httpClientFactory.CreateClient();
            var dto = new
            {
                OtpCode = code.OtpCode,
                Email = email
            };
            var response = await client.PostAsJsonAsync("https://localhost:7133/api/Account/VerifyCode", dto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiVerifyCodeResponse>();
                TempData["Email"] = result.Email;
                TempData["Token"] = result.ResetToken;
                return RedirectToAction("ResetPasswordPage");
            }

            ViewBag.Error = "Invalid verification code";
            return View();
        }
        public IActionResult ResetPasswordPage()
        {
            var model = new ResetPasswordViewModel();
            model.Email = TempData["Email"]?.ToString();
            model.Token = TempData["Token"]?.ToString();
            TempData.Keep("Email");

            TempData.Keep("Token");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string NewPassword, string ConfirmPassword)
        {
            var newPassword = new ResetPasswordViewModel();
            newPassword.ConfirmPassword = ConfirmPassword;
            newPassword.NewPassword = NewPassword;
            // 🚨 الخطوة 1: تشخيص حالة التحقق 🚨
            if (!ModelState.IsValid)
            {
                // إذا دخل الكود إلى هنا، فالمشكلة هي التحقق من الصحة (Validation)
                TempData["Error"] = "Validation failed. Please review the form data.";
                return View("ResetPasswordPage", newPassword);
            }

            // إذا وصل الكود إلى ما بعد هذا الشرط، فـ NewPassword و ConfirmPassword ستكونان مملوءتين.

            // 2. إذا نجح التحقق، أكمل خطتك
            string email = newPassword.Email;
           

            // ... أكمل منطق سحب TempData والاتصال بالـ API كما شرحنا سابقاً ...

            if (string.IsNullOrEmpty(email))
            {
                newPassword.Email = TempData["Email"]?.ToString();
                newPassword.Token = TempData["Token"]?.ToString();
            }

            if (string.IsNullOrEmpty(newPassword.Email) || string.IsNullOrEmpty(newPassword.Token))
                return RedirectToAction("ForgetPasswordRequest");


            var client = _httpClientFactory.CreateClient();
            var resetDto = new { Email = newPassword.Email, Token = newPassword.Token, NewPassword = newPassword.NewPassword };
            var content = new StringContent(JsonConvert.SerializeObject(resetDto), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7133/api/Account/ResetPassword", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Password has been reset successfully.";
                return RedirectToAction("Login");
            }

            TempData["Error"] = "Failed to reset password.";
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            // امسح الكوكي اللي فيها التوكن
            Response.Cookies.Delete("AuthToken");

            //// ممكن كمان تمسح أي Session لو مستخدم
            //HttpContext.Session.Clear();

            // رجّع المستخدم لصفحة اللوجين
            return RedirectToAction("Index", "Mastar");
        }




        [HttpGet]
        public async Task<IActionResult> GetAddresses()
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login");

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("https://centralwebsite.runasp.net/api/Account/GetAddresses");
            if (!response.IsSuccessStatusCode) return RedirectToAction("Login");

            var json = await response.Content.ReadAsStringAsync();
            var addresses = JsonConvert.DeserializeObject<List<ShippingAddressViewModel>>(json);

            return Json(addresses); // أو PartialView إذا هتستخدم Popup
        }

        // POST: إضافة عنوان جديد
        [HttpPost]
        public async Task<IActionResult> AddAddress([FromBody] ShippingAddressCreateViewModel newAddress)
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login");

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonConvert.SerializeObject(newAddress), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://centralwebsite.runasp.net/api/Account/AddAddresses", content);

            if (!response.IsSuccessStatusCode) return BadRequest("Failed to add address");

            return Json(newAddress);
        }

        // POST: تعديل عنوان موجود
        [HttpPost]
        public async Task<IActionResult> EditAddress(ShippingAddressCreateViewModel updatedAddress)
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login");

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


            var content = new StringContent(JsonConvert.SerializeObject(updatedAddress), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"https://centralwebsite.runasp.net/api/Account/EditAddress", content);

            if (!response.IsSuccessStatusCode) return BadRequest("Failed to update address");

            return RedirectToAction("Index");
        }

        // POST: حذف عنوان
        [HttpPost]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var token = Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login");

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync($"https://centralwebsite.runasp.net/api/Account/DeleteAddress/{id}");
            if (!response.IsSuccessStatusCode) return BadRequest("Failed to delete address");

            return RedirectToAction("Index");
        }
    }

    //        [HttpGet]
    //        public IActionResult DebugAuth()
    //        {
    //            string Mask(string s)
    //            {
    //                if (string.IsNullOrEmpty(s)) return "(empty)";
    //                var start = s.Length >= 12 ? s.Substring(0, 12) : s;
    //                var end = s.Length > 8 ? s.Substring(s.Length - 8) : "";
    //                return $"{start}...{end} (len={s.Length})";
    //            }

    //            var cookieToken = Request.Cookies["AuthToken"];

    //            var report =
    //        $@"Cookie Present: {(string.IsNullOrEmpty(cookieToken) ? "❌" : "✅")}  
    //{(cookieToken is null ? "" : Mask(cookieToken))}";

    //            return Content(report, "text/plain");
    //        }


    //[HttpPost]
    //public async Task<IActionResult> ForgetPasswordRequest(string email)
    //{
    //    var client = _httpClientFactory.CreateClient();
    //    var requestDto = new { Email = email };
    //    var content = new StringContent(JsonConvert.SerializeObject(requestDto), Encoding.UTF8, "application/json");

    //    var response = await client.PostAsync("https://localhost:7133/api/Account/forgetpassword", content);

    //    if (response.IsSuccessStatusCode)
    //    {
    //        TempData["Email"] = email;
    //        TempData["Message"] = "Check your email for the reset link.";
    //        return RedirectToAction("SendVerificationCodePage");
    //    }

    //    TempData["Error"] = "Email not found or failed to send.";
    //    return View();
    //}

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Logout()
    //{
    //    await _signInManager.SignOutAsync();
    //    return RedirectToAction("Index", "Home");
    //}

}

