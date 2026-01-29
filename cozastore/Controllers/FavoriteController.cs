using AccountDAL.Eentiti;
using AccountDAL.Eentiti.CozaStore;
using AccountDAL.Repositories;
using AccountDAL.Specifications.Favorite;
using AutoMapper;
using cozastore.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cozastore.Controllers
{
    public class FavoriteController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FavoriteController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        //إضافة منتج للمفضلة
        [HttpPost]
        public async Task<IActionResult> AddFavorite( int productId)
        {
            // استخرج اسم المستخدم من التوكن
            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { message = "المستخدم غير مسجل دخول" });
            if (!await _unitOfWork.Repository<Favorites>().IsFavoriteAsync(userName, productId))
            {
                var favorite = new Favorites { UserEmail = userName, ProductId = productId };
                await _unitOfWork.Repository<Favorites>().AddAsync(favorite);
                await _unitOfWork.Complete();
            }
            return Ok(new { message = "تمت الإضافة للمفضلات" });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFavorite( int productId)
        {
            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { message = "المستخدم غير مسجل دخول" });
            var favorite = await _unitOfWork.Repository<Favorites>().GetFavoriteAsync(userName, productId);
            if (favorite != null)
            {
                _unitOfWork.Repository<Favorites>().Delete(favorite);
                await _unitOfWork.Complete();
                return Ok(new { message = "تم الحذف من المفضلات" });
            }
            return NotFound(new { message = "المنتج غير موجود في المفضلات" });
        }

        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            var userName = User.Identity?.Name;
            var rqf = HttpContext.Features.Get<IRequestCultureFeature>();
            var lang = rqf?.RequestCulture.UICulture.TwoLetterISOLanguageName ?? "en";
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { message = "المستخدم غير مسجل دخول" });
            var spec = new GetFavoritesByUserAsyncSpecification(userName);
            var favorites = await _unitOfWork.Repository<Favorites>().GetAllWithSpecAsync(spec);
            var DataAfterMap = _mapper.Map<IReadOnlyList<FavoritesViewModel>>(favorites, opt =>
            {
                opt.Items["lang"] = "ar"; // أو "en" حسب اللغة
            });
            return View(DataAfterMap);
        }

        [HttpGet]
        public async Task<IActionResult> IsFavorite( int productId)
        {
            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized();
            var isFav = await _unitOfWork.Repository<Favorites>().IsFavoriteAsync(userName, productId);
            return Ok(new { productId, isFav });
        }
    }
}
