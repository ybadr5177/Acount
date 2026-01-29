using AccountDAL.Eentiti.CozaStore;
using AccountDAL.config;
using AccountDAL.Eentiti;
using AccountDAL.Repositories;
using AutoMapper;
using DashBoard.Helpers;
using DashBoard.ViewModel.CozaMaster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
using System.Drawing;
using System.Formats.Asn1;

namespace DashBoard.Controllers
{
    public class CozaMasterController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly CozaStoreContext _cozaContext;

        public CozaMasterController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager, CozaStoreContext cozaContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _cozaContext = cozaContext;
        }
        public async Task<IActionResult> HomePage()
        {
           
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersCount()
        {
            var count = await _userManager.Users.CountAsync();
            return Json(new { count });
        }

        [HttpGet]
        public async Task<IActionResult> GetOrdersCount()
        {
            var count = await _cozaContext.Orders.CountAsync();
            return Json(new { count });
        }

        [HttpGet]
        public async Task<IActionResult> GetTotals()
        {
            var totalUsers = await _userManager.Users.CountAsync();
            var totalOrders = await _cozaContext.Orders.CountAsync();
            var totalDeliveries = await _cozaContext.DeliveryCosts.CountAsync();
            var totalProducts = await _cozaContext.Products.CountAsync();
            return Json(new { users = totalUsers, orders = totalOrders, deliveries = totalDeliveries, products = totalProducts });
        }

        [HttpGet]
        public async Task<IActionResult> GetOrdersDaily(int days = 30)
        {
            if (days < 1) days = 1;
            var end = DateTime.UtcNow.Date;
            var start = end.AddDays(-(days - 1));

            var series = await _cozaContext.Orders
                .Where(o => o.OrderDate >= start && o.OrderDate < end.AddDays(1))
                .GroupBy(o => EF.Functions.DateDiffDay(start, o.OrderDate))
                .Select(g => new { day = g.Key, count = g.Count() })
                .ToListAsync();

            var labels = Enumerable.Range(0, days).Select(i => start.AddDays(i)).ToList();
            var data = labels.Select((d, i) => series.FirstOrDefault(s => s.day == i)?.count ?? 0).ToList();

            var totalAllTime = await _cozaContext.Orders.CountAsync();
            var startMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var thisMonth = await _cozaContext.Orders.CountAsync(o => o.OrderDate >= startMonth);
            var today = await _cozaContext.Orders.CountAsync(o => o.OrderDate >= end && o.OrderDate < end.AddDays(1));

            return Json(new
            {
                labels = labels.Select(d => d.ToString("yyyy-MM-dd")),
                data,
                stats = new { totalAllTime, thisMonth, today }
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersDaily(int days = 30)
        {
            // AppUser has no CreatedAt. Provide totals and zero daily sequence for UI consistency.
            if (days < 1) days = 1;
            var end = DateTime.UtcNow.Date;
            var start = end.AddDays(-(days - 1));
            var labels = Enumerable.Range(0, days).Select(i => start.AddDays(i)).ToList();

            var totalAllTime = await _userManager.Users.CountAsync();
            var thisMonth = 0;
            var today = 0;

            return Json(new
            {
                labels = labels.Select(d => d.ToString("yyyy-MM-dd")),
                data = labels.Select(_ => 0),
                stats = new { totalAllTime, thisMonth, today }
            });
        }

        #region Categrey
        public async Task<IActionResult> Categrey()
        {
            var date = await _unitOfWork.Repository<Category>().GetAllAsync();
            var mapData = _mapper.Map<IEnumerable<CategoryViewModel>>(date);
            return View(mapData);
        }



        #region Creat
        public async Task<IActionResult> CreatCategrey()
        {
            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreatCategrey(CategoryViewModel categreyViewModel)
        {
            if (categreyViewModel.PictureName == null)
            {
                return View(categreyViewModel);
            }
            categreyViewModel.Picture = DocumentSettings.UploudFile(categreyViewModel.PictureName, "image");

            //if (ModelState.IsValid)
            //{
                var data = _mapper.Map<CategoryViewModel, Category>(categreyViewModel);
                await _unitOfWork.Repository<Category>().AddAsync(data);
                await _unitOfWork.Complete();
                var data1 = _mapper.Map<Category, CategoryViewModel>(data);

                return View(data1);
            //}


            //foreach (var entry in ModelState)
            //{
            //    foreach (var error in entry.Value.Errors)
            //    {
            //        Console.WriteLine($"Field: {entry.Key} => Error: {error.ErrorMessage}");
            //    }
            //}
            DocumentSettings.DeleteFile("image", categreyViewModel.Picture);

            return View(categreyViewModel);
        }
        #endregion
        #region Edit
        public async Task<IActionResult> EditCategrey(int id)
        {
            var data = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
            var category = _mapper.Map<Category, CategoryViewModel>(data);


            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> EditCategrey(CategoryViewModel categreyViewModel)
        {
            if (ModelState.IsValid)
            {
                var data = _mapper.Map<CategoryViewModel, Category>(categreyViewModel);
                _unitOfWork.Repository<Category>().Update(data);
                await _unitOfWork.Complete();
            }
            return View();
        }
        #endregion

        #region Delete
        public async Task<IActionResult> DeleteCategrey(int id)
        {
            var data = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
            var category = _mapper.Map<Category, CategoryViewModel>(data);


            return View(category);

        }
        [HttpPost]
        public async Task<IActionResult> DeleteCategreyfn(int id)
        {
            if (ModelState.IsValid)
            {
                var data = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
                //var data1 = _mapper.Map<CategoryViewModel, Category>(categreyViewModel);
                DocumentSettings.DeleteFile("image", data.Picture);
                _unitOfWork.Repository<Category>().Delete(data);
                await _unitOfWork.Complete();
            }
            return View();
        }
        #endregion

        #endregion


        #region Slider
        public IActionResult GetAllSlider()
        {
            return View();
        }
        [HttpGet]
        public IActionResult CreatSlider()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreatSlider(SliderViewModel sliderModel)
        {
            sliderModel.ImagePath = DocumentSettings.UploudFile(sliderModel.Image, "image");
            var mapsl = _mapper.Map<SliderViewModel, SliderItem>(sliderModel);

            _unitOfWork.Repository<SliderItem>().AddAsync(mapsl);
            await _unitOfWork.Complete();

            return RedirectToAction("GetAllSlider");
        }
        #endregion


        #region DiscountCodes
        public async Task<IActionResult> GetAllDiscountCodes()
        {
            var data = await _unitOfWork.Repository<opponent>().GetAllAsync();



            return View(data);
        }


        #region Creat
        [HttpGet]
        public IActionResult CreatDiscountCodes()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreatDiscountCodes(DiscountCodesViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var Data = _mapper.Map<DiscountCodesViewModel, opponent>(model);
            await _unitOfWork.Repository<opponent>().AddAsync(Data);
            await _unitOfWork.Complete();
            return View(Data);
        }
        #endregion
        #region Edit
        public async Task<IActionResult> EditDiscountCodes(int id)
        {
            var data = await _unitOfWork.Repository<opponent>().GetByIdAsync(id);
            var DiscountCodes = _mapper.Map<opponent, DiscountCodesViewModel>(data);


            return View(DiscountCodes);
        }
        [HttpPost]
        public async Task<IActionResult> EditDiscountCodes(DiscountCodesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var data = _mapper.Map<DiscountCodesViewModel, opponent>(model);
                _unitOfWork.Repository<opponent>().Update(data);
                await _unitOfWork.Complete();
            }


            return View();
        }
        #endregion
        #region Delete
        public async Task<IActionResult> DeleteDiscountCodes(int id)
        {
            var data = await _unitOfWork.Repository<opponent>().GetByIdAsync(id);
            var opponent = _mapper.Map<opponent, DiscountCodesViewModel>(data);


            return View(opponent);

        }
        [HttpPost]
        public async Task<IActionResult> DeleteDiscountCodesfn(int id)
        {
            if (ModelState.IsValid)
            {
                var data = await _unitOfWork.Repository<opponent>().GetByIdAsync(id);
                _unitOfWork.Repository<opponent>().Delete(data);
                await _unitOfWork.Complete();
            }
            return View();
        }
        #endregion
        #endregion
    }
}
