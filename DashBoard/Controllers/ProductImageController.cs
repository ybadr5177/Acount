using AccountDAL.Eentiti.CozaStore;
using AccountDAL.Repositories;
using AccountDAL.Specifications.ProductImages;
using AutoMapper;
using DashBoard.Helpers;
using DashBoard.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace DashBoard.Controllers
{
    public class ProductImageController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ProductImageController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IActionResult> DeletProductImages(int id)
        {
            var specData = new PicturesbytheproductSpec(id);
            var data = await _unitOfWork.Repository<ProductImage>().GetByIdWithSpecAsync(specData);

            var MapData = _mapper.Map<ProductImagesViewModel>(data);
            return View(MapData);
        }
        [HttpPost]
        public async Task< IActionResult> DeletProductImagesfn(int id)
        {
            var specData = new PicturesbytheproductSpec(id);
            var data = await _unitOfWork.Repository<ProductImage>().GetByIdWithSpecAsync(specData);
            DocumentSettings.DeleteFile("image", data.ImageUrl);
            _unitOfWork.Repository<ProductImage>().Delete(data);
            await _unitOfWork.Complete();

            return View();
        }
        public async  Task< IActionResult> GetAllProductImages(int id)
        {
            var specData = new PicturesbytheproductSpec(id);
            var data= await _unitOfWork.Repository<ProductImage>().GetAllWithSpecAsync(specData);

             var MapData = _mapper.Map<List<ProductImagesViewModel>>(data);
            return View(MapData);
        }
        [HttpGet]
        public IActionResult AddProductImages()
        {
            return View();
        }
        public async Task< IActionResult> AddProductImages(AddProductImagesViewModel model)
        {


            model.ImageUrl = DocumentSettings.UploudFile(model.Image, "image");

            var productImage = _mapper.Map<AddProductImagesViewModel, ProductImage>(model);

            await _unitOfWork.Repository<ProductImage>().AddAsync(productImage);
            await _unitOfWork.Complete();
            return View();
        }
    }
}
