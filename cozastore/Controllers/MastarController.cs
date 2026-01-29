using AccountDAL.Eentiti.CozaStore;
using AccountDAL.Repositories;
using AccountDAL.Specifications.ProductsSpecification;
using AccountDAL.Specifications.SubCategorys;
using AutoMapper;
using BAL.Service;
using cozastore.Helpers;
using cozastore.ViewModel;
using cozastore.ViewModel.CozaMaster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.VisualBasic;

namespace cozastore.Controllers
{
    public class MastarController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<MastarController> _Localizer;


        public MastarController(IUnitOfWork unitOfWork, IMapper mapper, IStringLocalizer<MastarController> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _Localizer = localizer;
        }

        public async Task<IActionResult> Index(ProductSpecParams? param)
        {
            var rqf = HttpContext.Features.Get<IRequestCultureFeature>();
            var lang = rqf?.RequestCulture.UICulture.TwoLetterISOLanguageName ?? "en";
            //var lang = CurrentLang;
            //param ??= new ProductSpecParams();
            var CategreyDate = await _unitOfWork.Repository<Category>().GetAllAsync();
            var spec = new ProductWithSubCategorySpecifications(param);
            var Product = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);
            var PMap = _mapper.Map<List<ProductDisplayViewModel>>(Product, opt =>
            {
                opt.Items["lang"] = lang;
            });
            var totalData = _mapper.Map<ProductPageViewModel>((param, PMap, CategreyDate));
            // ✅ لو الطلب Ajax رجّع Partial
            //if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            //{
            //    return PartialView("_ProductsPartial", PMap);
            //}
            return View(totalData);
        }
        [HttpGet]
        public async Task<IActionResult> ProductsPartial([FromQuery] ProductSpecParams? param)
        {
            var rqf = HttpContext.Features.Get<IRequestCultureFeature>();
            var lang = rqf?.RequestCulture.UICulture.TwoLetterISOLanguageName ?? "en";
            var CategreyDate = await _unitOfWork.Repository<Category>().GetAllAsync();
            var spec = new ProductWithSubCategorySpecifications(param);
            var countSpec = new ProductWithFiltersForCountSpecification(param);
            var totalItems = await _unitOfWork.Repository<Product>().GetCountAsync(countSpec);
            var Product = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);
            var PMap = _mapper.Map<List<ProductDisplayViewModel>>(Product, opt =>
            {
                opt.Items["lang"] = lang;
            });
            var totalData = _mapper.Map<ProductPageViewModel>((param, PMap, CategreyDate));
            //نرسل البيانات3//
            //var pagination = new Pagination<ProductDisplayViewModel>(
            //    param.PageIndex,
            //      param.PageSize,
            //     totalItems,
            //     PMap
            //        );
            return PartialView("ProductsPartial", PMap);
        }
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(30) }
                );

            return LocalRedirect(returnUrl);
        }


   


        public async Task<IActionResult> GetAllCategory()
        {
            var rqf = HttpContext.Features.Get<IRequestCultureFeature>();
            var lang = rqf?.RequestCulture.UICulture.TwoLetterISOLanguageName ?? "en";

            var category = await _unitOfWork.Repository<Category>().GetAllAsync();


            return View(category);
        }

        public async Task<IActionResult> GetAllSubCategory(int id)
        {
            var rqf = HttpContext.Features.Get<IRequestCultureFeature>();
            var lang = rqf?.RequestCulture.UICulture.TwoLetterISOLanguageName ?? "en";
            var spec = new SubCategoryWithCategory(id);
            var category = await _unitOfWork.Repository<SubCategory>().GetAllWithSpecAsync(spec);
            var data = _mapper.Map<IEnumerable<SubCategoryViewModel>>(category,opt =>
            {
                opt.Items["lang"] = lang;
            });
            return View(data);
        }

        public async Task<IActionResult> Sliders()
        {
            var rqf = HttpContext.Features.Get<IRequestCultureFeature>();
            var lang = rqf?.RequestCulture.UICulture.TwoLetterISOLanguageName ?? "en";
            var sliders = await _unitOfWork.Repository<SliderItem>().GetAllAsync();
            var viewModels = _mapper.Map<List<SliderItemViewModel>>(sliders, opt =>
            {
                opt.Items["lang"] = lang;
            });
            return View(viewModels);
        }
        public async Task<IActionResult> GetProductBySubCategory(ProductSpecParams? param)
        {
            var rqf = HttpContext.Features.Get<IRequestCultureFeature>();
            var lang = rqf?.RequestCulture.UICulture.TwoLetterISOLanguageName ?? "en";
            //param ??= new ProductSpecParams();
            var specCategry = new SubCategoryWithCategory(param.CategoryId ?? 0);
            var CategreyDate = await _unitOfWork.Repository<SubCategory>().GetAllWithSpecAsync(specCategry);
            var CategreyMap = _mapper.Map <List< SubCategoryViewModel>> (CategreyDate, opt =>
            {
                opt.Items["lang"] = lang;
            });
            var spec = new ProductWithSubCategorySpecifications(param);
            var Product = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);
            var PMap = _mapper.Map<List<ProductDisplayViewModel>>(Product, opt =>
            {
                opt.Items["lang"] = lang;
            });
            var totalData = _mapper.Map<ProductAndSubCategoryPageViewModel>((param, PMap, CategreyMap));
            return View(totalData);
        }
        public async Task<IActionResult> ProductsUsingSubCategory(ProductSpecParams? param)
        {
            var rqf = HttpContext.Features.Get<IRequestCultureFeature>();
            var lang = rqf?.RequestCulture.UICulture.TwoLetterISOLanguageName ?? "en";

            var spec = new ProductWithSubCategorySpecifications(param);
            var Product = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);
            var PMap = _mapper.Map<List<ProductDisplayViewModel>>(Product, opt => { opt.Items["lang"] = lang; });
            var totalData = _mapper.Map<ProductUsingSubCategoryPageViewModel>((param, PMap));
            return View(totalData);
        }

     
        public async Task<IActionResult> DetailsProduct(int Id)
        {
            var rqf = HttpContext.Features.Get<IRequestCultureFeature>();
            var lang = rqf?.RequestCulture.UICulture.TwoLetterISOLanguageName ?? "en";
            var spec = new ProductWithProductSizeByIdSpecifications(Id);
            var Product = await _unitOfWork.Repository<Product>().GetByIdWithSpecAsync(spec);
            var totalData = _mapper.Map<Product, DetailsProductDisplayViewModel>(Product, opt =>
            {
                opt.Items["lang"] = lang;
            });
            return View(totalData);
        }
        [HttpPost]
        public async Task<IActionResult> FindTheDiscountCode([FromBody] string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return Json(new { isValid = false, amount = 0 });

            var product = await _unitOfWork.Repository<opponent>()
                .findStringAsync(s => s.Codes, code);

            if (product == null)
                return Json(new { isValid = false, amount = 0 });

            decimal discount = product.DiscountPercentage;

            return Json(new { isValid = true, amount = discount, id = product.Id });
        }
    }
}
