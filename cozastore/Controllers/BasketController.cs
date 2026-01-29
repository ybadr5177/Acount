using AccountDAL.Eentiti;
using AccountDAL.Eentiti.CozaStore;
using AccountDAL.Repositories;
using AccountDAL.Specifications.ProductSizes;
using AccountDAL.Specifications.ProductsSpecification;
using AutoMapper;
using cozastore.ViewModel;
using cozastore.ViewModel.CozaMaster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace cozastore.Controllers
{
    public class BasketController : BaseController
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public BasketController(IBasketRepository basketRepository, IMapper mapper,IUnitOfWork unitOfWork)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        //[Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // تحديد هوية المستخدم (مسجل دخول أو Guest باستخدام Session)
            var userId = User.Identity.IsAuthenticated ? User.Identity.Name : HttpContext.Session.Id;

            // جلب السلة
            var basket = await _basketRepository.GetBasketAsync(userId);

            // لو السلة مش موجودة → نرجع ViewModel فاضي
            var viewModel = basket != null
                ? _mapper.Map<CustomerBasketViewModel>(basket)
                : new CustomerBasketViewModel(); // سلة فاضية

            return View(viewModel);
        }
        public async Task<IActionResult> GetBasket()
        {
            // تحديد هوية المستخدم (مسجل دخول أو Guest باستخدام Session)
            var userId = User.Identity.IsAuthenticated ? User.Identity.Name : HttpContext.Session.Id;

            // جلب السلة
            var basket = await _basketRepository.GetBasketAsync(userId);

            // لو السلة مش موجودة → نرجع ViewModel فاضي
            var viewModel = basket != null
                ? _mapper.Map<CustomerBasketViewModel>(basket)
                : new CustomerBasketViewModel(); // سلة فاضية

            return Json(viewModel);
        }
        
        [HttpPost]
        public async Task<IActionResult> AddItem(ProductDisplayViewModel model, int sizeId)
        {

            string userId;
            if (User.Identity.IsAuthenticated)
            {
                userId = User.Identity.Name;
            }
            else
            {
                //// تحقق من وجود معرف جلسة
                //if (string.IsNullOrEmpty(HttpContext.Session.Id))
                //{
                //    // إذا لم يوجد، قم بتوليد معرف فريد جديد
                //    HttpContext.Session.SetString("BasketId", Guid.NewGuid().ToString());
                //}
                //userId = HttpContext.Session.GetString("BasketId");
                // إعادة التوجيه لصفحة تسجيل الدخول
                return RedirectToAction("login", "Account");
            }

            // 2. الآن يمكنك استخدام userId الذي تم التأكد من وجوده
            var basket = await _basketRepository.GetBasketAsync(userId) ?? new CustomerBasket(userId);
            //var userId = User.Identity.IsAuthenticated ? User.Identity.Name : HttpContext.Session.Id;
            //var basket = await _basketRepository.GetBasketAsync(userId) ?? new CustomerBasket(userId);

            var specProduct = new ProductWithProductSizeByIdSpecifications(model.Id);

            var product = await _unitOfWork.Repository<Product>().GetByIdWithSpecAsync(specProduct);
            var specProductSize = new ProductSizeByIdWithSizeSpecifications(sizeId);
            var productSize = await _unitOfWork.Repository<ProductSize>().GetByIdWithSpecAsync(specProductSize);
            var rqf = HttpContext.Features.Get<IRequestCultureFeature>();
            var lang = rqf?.RequestCulture.UICulture.TwoLetterISOLanguageName ?? "en";

            var allProdect =  _mapper.Map<Product, ProductDisplayViewModel>(product, opts =>
            {
                opts.Items["lang"] = lang;
            });
            var PictureUrl = allProdect.Picture.FirstOrDefault();

            if (product == null) return NotFound();

            var existingItem = basket.Items.FirstOrDefault(x => x.Id == model.Id /*&& x.ProductSize == model.ProductSize.*/);

            if (existingItem != null)
            {
                existingItem.Quantity += model.Quantity;
            }
            else
            {
                basket.Items.Add(new BasketItem
                {
                    Id = product.Id,
                    ProductName = product.Address_En,
                    ProductDescription = product.description_En,
                    Price = productSize.DiscountPrice == 0 ? productSize.Price : productSize.DiscountPrice,
                    Quantity = model.Quantity,
                    Size = productSize.Size.Label,
                    PictureUrl = PictureUrl,
                    CategoryorSubCategory = product.Category?.Name_EN
                });
            }

            await _basketRepository.UpdateBasketAsync(basket);
            return RedirectToAction("Index");
        }

        // تعديل كمية
        [HttpPost]
        public async Task<IActionResult> UpdateItemQuantity(int productId, string size, int quantity)
        {
            var userId = User.Identity.IsAuthenticated ? User.Identity.Name : HttpContext.Session.Id;
            var basket = await _basketRepository.GetBasketAsync(userId);
            if (basket == null) return NotFound();

            var item = basket.Items.FirstOrDefault(x => x.Id == productId && x.ProductSize == size);
            if (item != null)
            {
                item.Quantity = quantity;
                await _basketRepository.UpdateBasketAsync(basket);
            }

            return RedirectToAction("Index");
        }

        // حذف منتج
        [HttpPost]
        public async Task<IActionResult> RemoveItem([FromBody]RemoveItemRequest removeItemRequest)
        {
            var userId = User.Identity.IsAuthenticated ? User.Identity.Name : HttpContext.Session.Id;
            var basket = await _basketRepository.GetBasketAsync(userId);
            if (basket == null) return NotFound();

            basket.Items.RemoveAll(x => x.Id == removeItemRequest.ProductId && x.Size == removeItemRequest.Size);
            await _basketRepository.UpdateBasketAsync(basket);

            return RedirectToAction("Index");
        }

        // مسح السلة
        [HttpPost]
        public async Task<IActionResult> ClearBasket()
        {
            var userId = User.Identity.IsAuthenticated ? User.Identity.Name : HttpContext.Session.Id;
            await _basketRepository.DeleteBasketAsync(userId);
            return RedirectToAction("Index");
        }
        //[HttpGet]
        //public async Task<IActionResult> GetBasketById(string id)
        //{
        //    var basket = await _basketRepository.GetBasketAsync(id);
        //    if(basket == null)
        //    {
        //        basket = new CustomerBasket(id);
        //           return View(basket);
        //    }

        //    return View(basket);
        //}

        //[HttpPost]
        //public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketViewModel basket)
        //{
        //    var mappedBasket = _mapper.Map<CustomerBasketViewModel, CustomerBasket>(basket);
        //    var updatedBasket = await _basketRepository.UpdateBasketAsync(mappedBasket);
        //    return Ok(updatedBasket);
        //}

        //[HttpPost]
        //public async Task DeleteBasket(string id)
        //{
        //    await _basketRepository.DeleteBasketAsync(id);
        //}



        [HttpGet]
        public async Task<IActionResult> payview(int? discountId)
        {
            // تحديد هوية المستخدم (مسجل دخول أو Guest باستخدام Session)
            var userId = User.Identity.IsAuthenticated ? User.Identity.Name : HttpContext.Session.Id;

            // جلب السلة
            var basket = await _basketRepository.GetBasketAsync(userId);

            // لو السلة مش موجودة → نرجع ViewModel فاضي
            var viewModel = basket != null
                ? _mapper.Map<CustomerBasketViewModel>(basket)
                : new CustomerBasketViewModel(); // سلة فاضية

            return View(viewModel);
        }
    }
}
