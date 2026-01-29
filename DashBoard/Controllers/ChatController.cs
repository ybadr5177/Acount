using AccountDAL.Services;
using DashBoard.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DashBoard.Controllers
{
    public class ChatController : Controller
    {
        private readonly IMessageService _messageService;

        // حقن خدمة الرسائل (التي تستخدم Unit of Work)
        public ChatController(IMessageService messageService)
        {
            _messageService = messageService;
        }
        public async Task<IActionResult> Index()
        {
            // جلب قائمة المحادثات لملء الجزء الأيمن من الداش بورد
            // سنستخدم دالة GetAllActiveConversationsAsync التي أضفناها لـ IMessageService
            var conversations = await _messageService.GetAllActiveConversationsAsync();

            // تمرير قائمة المحادثات إلى View
            return View(conversations);
        }

        // -----------------------------------------------------------
        // 2. Action: UploadFile (نقطة نهاية AJAX لرفع الملفات)
        // -----------------------------------------------------------
        // لا ترجع View، بل ترجع JSON لـ JavaScript
        [HttpPost]
        public IActionResult UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Json(new { success = false, message = "No file selected." });
            }

            const string ChatFilesFolder = "chat";

            try
            {
                // استدعاء دالة الرفع الجاهزة لديك
                string fileName = DocumentSettings.UploudFile(file, ChatFilesFolder);

                // بناء المسار العام (URL)
                var fileUrl = $"/files/{ChatFilesFolder}/{fileName}";

                // إرجاع الـ URL للعميل ليرسله عبر SignalR
                return Json(new { success = true, fileUrl = fileUrl });
            }
            catch (Exception ex)
            {
                // في حال فشل الرفع لأي سبب
                // هنا يجب عليك أيضاً تسجيل الخطأ
                return StatusCode(500, Json(new { success = false, message = "File upload failed.", error = ex.Message }));
            }
        }

        // -----------------------------------------------------------
        // 3. Action: LoadHistory (لتحميل سجل المحادثة عند النقر)
        // -----------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> LoadHistory(int conversationId)
        {
            if (conversationId <= 0)
            {
                return BadRequest("Invalid conversation ID.");
            }
          var adminUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "DEV_ADMIN_USER_ID_12345";
    
                  // 🚨 إضافة الـ Admin ID إلى ViewData ليتم استخدامه في الـ View للمقارنة
               ViewData["AdminId"] = adminUserId;
            // جلب سجل الرسائل القديم والمرتب زمنياً
            var messages = await _messageService.GetConversationMessagesAsync(conversationId);

            // نرجع سجل الرسائل كـ Partial View لملء منطقة الدردشة
            return PartialView("_ChatMessagePartial", messages);
        }
        //[HttpGet]
        //[Authorize] // 💡 تأكد من أن هذا End-point يتطلب المصادقة
        //public IActionResult GetAdminSignalRInfo()
        //{
        //    // الحصول على الـ User ID للمستخدم الحالي المصادق عليه عبر Identity Cookie
        //    var adminUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    if (string.IsNullOrEmpty(adminUserId))
        //    {
        //        // إذا لم يكن المستخدم مصادقاً (وهو أمر لا يجب أن يحدث إذا كان [Authorize] يعمل)
        //        return Unauthorized();
        //    }

        //    // هنا نعود بـ UserID في جسم الرد (Response Body)، وهو ما سنستخدمه كـ "توكن" للـ Hub
        //    // SignalR Hub سيستخدم هذا الـ UserID لمصادقة المستخدم.
        //    return Ok(new { senderId = adminUserId });
        //}

        [HttpGet]
        [AllowAnonymous] // 🚨 مهم: السماح بالوصول بدون مصادقة مؤقتاً
        public IActionResult GetAdminSignalRInfo()
        {
            // 🚨 قيمة ID ثابتة للاختبار أثناء التطوير
            var tempAdminUserId = "DEV_ADMIN_USER_ID_12345";

            // قم بتغيير هذا المسار عندما تبدأ بإنشاء الصلاحيات
            // إذا كان المستخدم مصدقاً بالفعل، يمكنك استخدام الآتي (الوضع النهائي):
            /*
            if (User.Identity.IsAuthenticated)
            {
                var adminUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return Ok(new { senderId = adminUserId });
            }
            */

            return Ok(new { senderId = tempAdminUserId });
        }
    }
}
