using AccountDAL.Eentiti.CozaStore;
using AccountDAL.Repositories;
using AccountDAL.Specifications.SubCategorys;
using AutoMapper;
using DashBoard.Helpers;
using DashBoard.ViewModel.CozaMaster;
using Microsoft.AspNetCore.Mvc;

namespace DashBoard.Controllers
{
    public class SubCategreyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SubCategreyController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IActionResult> GetAllSubCategrey()
        {
            var spec = new GetAllSubCategoryWithCategory();
            var category = await _unitOfWork.Repository<SubCategory>().GetAllWithSpecAsync(spec);
            var DataView = _mapper.Map<IReadOnlyList<SubCategory>, IReadOnlyList<GetAllSubCategoryViewModel>>(category);


            return View(DataView);
        }


        #region Creat
        public async Task<IActionResult> CreatSubCategrey()
        {
            var CategreyDate = await _unitOfWork.Repository<Category>().GetAllAsync();


            var viewModel = new SubCategoryViewModel
            {
                Categories = _mapper.Map<List<CategoryViewModel>>(CategreyDate),

            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreatSubCategrey(SubCategoryViewModel subViewModel)
        {
            var testCategory = await _unitOfWork.Repository<Category>().ExistsAsync(c => c.Id == subViewModel.CategoryId);
            if (!testCategory)
            {
                var categories = await _unitOfWork.Repository<Category>().GetAllAsync();
                subViewModel.Categories = _mapper.Map<List<CategoryViewModel>>(categories);
                return View(subViewModel);
            }
            subViewModel.Picture = DocumentSettings.UploudFile(subViewModel.PictureName, "image");

            var Data = _mapper.Map<SubCategoryViewModel, SubCategory>(subViewModel);
            await _unitOfWork.Repository<SubCategory>().AddAsync(Data);
            await _unitOfWork.Complete();
            return RedirectToAction("GetAllSubCategrey");
        }
        #endregion


        #region Edit
        public async Task<IActionResult> EditSubCategrey(int id)
        {
            var data = await _unitOfWork.Repository<SubCategory>().GetByIdAsync(id);
            var CategreyDate = await _unitOfWork.Repository<Category>().GetAllAsync();
            var category = _mapper.Map<SubCategory, SubCategoryViewModel>(data);


            category.Categories = _mapper.Map<List<CategoryViewModel>>(CategreyDate);





            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> EditSubCategrey(EditAndCreatSubCategoryViewModel categreyViewModel)
        {
            var datapicture = await _unitOfWork.Repository<SubCategory>().GetByIdAsync(categreyViewModel.Id);
            if (datapicture.Picture != categreyViewModel.Picture)
            {
                DocumentSettings.DeleteFile(datapicture.Picture, "image");
                categreyViewModel.Picture = DocumentSettings.UploudFile(categreyViewModel.PictureName, "image");

            }
            if (ModelState.IsValid)
            {

                var Data = _mapper.Map<EditAndCreatSubCategoryViewModel, SubCategory>(categreyViewModel);
                _unitOfWork.Repository<SubCategory>().Update(Data);
                await _unitOfWork.Complete();
               
            }
            //     var errors = ModelState
            //.Where(ms => ms.Value.Errors.Any())
            //.Select(ms => new
            //{
            //    Field = ms.Key,
            //    Errors = ms.Value.Errors.Select(e => e.ErrorMessage).ToList()
            //});

            //     // ممكن تطبعهم في اللوج أو ترجعهم في الـ response
            //     foreach (var error in errors)
            //     {
            //         Console.WriteLine($"Field: {error.Field}");
            //         foreach (var err in error.Errors)
            //         {
            //             Console.WriteLine($" - Error: {err}");
            //         }
            //     }

            //     return BadRequest(errors);

            return RedirectToAction("GetAllSubCategrey");
        }
        #endregion

        #region Delete
        public async Task<IActionResult> DeleteSubCategrey(int id)
        {
            var data = await _unitOfWork.Repository<SubCategory>().GetByIdAsync(id);
            var subCategory = _mapper.Map<SubCategory, SubCategoryViewModel>(data);


            return View(subCategory);

        }
        [HttpPost]
        public async Task<IActionResult> DeleteSubCategreyfn(int id)
        {
            if (ModelState.IsValid)
            {
                var data = await _unitOfWork.Repository<SubCategory>().GetByIdAsync(id);
                //var data1 = _mapper.Map<CategoryViewModel, Category>(categreyViewModel);
                DocumentSettings.DeleteFile("image", data.Picture);
                _unitOfWork.Repository<SubCategory>().Delete(data);
                await _unitOfWork.Complete();
            }
            return View();
        }
        #endregion
    }
}
