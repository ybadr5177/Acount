using AccountDAL.Eentiti.CozaStore;
using AccountDAL.Repositories;
using AccountDAL.Specifications.ProductsSpecification;
using AccountDAL.Specifications.Size;
using AutoMapper;
using DashBoard.Helpers;
using DashBoard.ViewModel.CozaMaster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Drawing;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace DashBoard.Controllers
{
    public class PredectCozaStoreController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public PredectCozaStoreController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        #region GetAllPredect
        public async Task<IActionResult> GetAllPredect(ProductSpecParams param)
        {
            var spec = new ProductWithSubCategorySpecifications(param);
            // إجمالي عدد المنتجات (بدون تطبيق Skip/Take)
            var countSpec = new ProductWithFiltersForCountSpecification(param);
            var totalItems = await _unitOfWork.Repository<Product>().GetCountAsync(countSpec);
            //نجيب المنتجات1//
            var AllData = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);
            //نعمل المابنج الخاص بيها //
            
            var DataForMap = _mapper.Map<List<productDisplayViewModel>>(AllData);
            //نرسل البيانات3//
              var pagination = new Pagination<productDisplayViewModel>(
                  param.PageIndex,
                    param.PageSize,
                   totalItems,
                   DataForMap
                      );


            return View(pagination);
        }

        public async Task<IActionResult> GetAllPredectForSlider()
        {
            
            var AllData = await _unitOfWork.Repository<Product>().GetAllAsync();
            var data = _mapper.Map<List<GetAllPredectForSlider>>(AllData);


            return Json(data);
        }
        #endregion
        #region View product details
        public async Task<IActionResult> ViewProductDetails(int id)
        {
            var spec = new ProductWithProductSizeByIdSpecifications(id);
            
            var AllData = await _unitOfWork.Repository<Product>().GetByIdWithSpecAsync(spec);
            
            var DataForMap = _mapper.Map<List<ProductSizeEntryViewModel>>(AllData.ProductSizes);
            

            return View(DataForMap);
        }
        #endregion

        #region Creat
        public async Task<IActionResult> CreatPredect()
        {
            var subCategreyData = await _unitOfWork.Repository<SubCategory>().GetAllAsync();
            var CategreyData = await _unitOfWork.Repository<Category>().GetAllAsync();
            var viewModel = new ProductViewModel
            {
                SubCategorys = _mapper.Map<List<SubCategoryViewModel>>(subCategreyData),
                Categorys = _mapper.Map<List<CategoryViewModel>>(CategreyData),

            };
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> CreatPredect([FromForm] ProductViewModel pmodel)
        {
            //var speclab = new GetIdSizeSpecifications(pmodel, pmodel.Type);
            //if (!ModelState.IsValid)
            //    return View(pmodel);
            var productSizesJson = Request.Form["ProductSizesJson"];
            pmodel.ProductSizes = JsonConvert.DeserializeObject<List<ProductSizeEntryViewModel>>(productSizesJson);
            //pmodel.ImageName = DocumentSettings.UploudFile(pmodel.PictureName, "image");
            var product = _mapper.Map<Product>(pmodel);
            //product.ProductImages = new List<ProductImage>();


            foreach (var image in pmodel.PictureName)
            {
                var Imagesr = DocumentSettings.UploudFile(image, "image");
                product.ImageName.Add(new ProductImage
                {
                    ImageUrl = Imagesr
                });
            }

            if (pmodel.BasePrice.HasValue)
            {
                product.BasePrice = pmodel.BasePrice.Value;
            }
            else
            {
                var minPrice = product.ProductSizes.Min(ps => ps.Price);
                product.BasePrice = minPrice;
            }

           
            try
            {
                await _unitOfWork.Repository<Product>().AddAsync(product);
               var changes= await _unitOfWork.Complete();
                if (changes > 0)
                {
                    return Ok(new { success = true, message = "✅ The Product has been saved successfully." });
                }
                else
                {
                    return BadRequest(new { success = false, message = "❌ No changes were saved." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving product: " + ex.ToString());
                // أو استخدم logger لتسجيل الخطأ
                return StatusCode(500, new { success = false, message = "⚠️ Error occurred while saving the product.", error = ex.Message });
            }

            //return RedirectToAction("GetAllPredect");
        }
        #endregion


        public async Task<IActionResult> EditPredect(int id)
        {
            //نجيب المنتجات1//
            var AllData = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            //نعمل المابنج الخاص بيها //
            var DataForMap = _mapper.Map<productDisplayViewModel>(AllData);
            //نرسل البيانات3//

            return View(DataForMap);
        }
        [HttpPost]
        public async Task<IActionResult> EditPredecten(ProductSpecParams param)
        {
            var spec = new ProductWithSubCategorySpecifications(param);
            //نجيب المنتجات1//
            var AllData = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);
            //نعمل المابنج الخاص بيها //
            var DataForMap = _mapper.Map<List<productDisplayViewModel>>(AllData);
            //نرسل البيانات3//

            return View(DataForMap);
        }
    }
}
