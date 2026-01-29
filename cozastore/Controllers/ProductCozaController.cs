using AccountDAL.Repositories;
using AccountDAL.Eentiti.CozaStore;
using AccountDAL.Specifications.ProductsSpecification;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace cozastore.Controllers
{
    public class ProductCozaController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ProductCozaController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

        }
        public async Task<IActionResult> GetAllProduct(ProductSpecParams param)
        {
            var spec = new ProductWithSubCategorySpecifications(param);
            var Product = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);
            return View(Product);
        }
        public async Task<IActionResult> DetailsProduct(int id)
        {
            var DetailsProduct = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            return View(DetailsProduct);
        }
    }
}
