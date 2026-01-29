using AccountDAL.Eentiti.CozaStore;
using AccountDAL.Repositories;
using AutoMapper;
using DashBoard.ViewModel.Accessories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DashBoard.Controllers
{
    public class AccessoriesDBController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccessoriesDBController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Feedback()
        {
            var data = await _unitOfWork.Repository<Feedback>().GetAllAsync();
            var DataMap = _mapper.Map<List<FeedBackViewModel>>(data);
            return View(DataMap);
        }
        [HttpPost]
        public async Task<IActionResult> FeedbackenDelet(int Id)
        {
            return RedirectToAction();
        }
        [HttpPost]
        public async Task<IActionResult> FeedbackenDeletfn(int Id)
        {
            var data = await _unitOfWork.Repository<Feedback>().GetByIdAsync(Id);
            _unitOfWork.Repository<Feedback>().Delete(data);
            await _unitOfWork.Complete();
            return RedirectToAction();
        }


        [HttpGet]
        public async Task<IActionResult> ContactUS()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ContactUS(ContactUSViewModel model)
        {
           

            var existing = await _unitOfWork.Repository<ContactUS>().GetFirstAsync();


            if (existing == null)
            {

                var DattaMap = _mapper.Map<ContactUSViewModel, ContactUS>(model);
                await _unitOfWork.Repository<ContactUS>().AddAsync(DattaMap);
            }
            else
            {
                _mapper.Map(model, existing);
                _unitOfWork.Repository<ContactUS>().Update(existing);
            }

            // حفظ التغييرات في قاعدة البيانات
            await _unitOfWork.Complete();
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> TermsAndConditions()
        {
            var lastData = await _unitOfWork.Repository<TermsAndConditions>().GetLatestAsync(x => x.LastUpdated);

            return View(lastData ?? new TermsAndConditions());
        }
        [HttpPost]
        public async Task<IActionResult> Edit(TermsAndConditions model)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Repository<TermsAndConditions>().AddAsync(model);
                await _unitOfWork.Complete();
                TempData["Success"] = "تم تحديث الشروط والأحكام بنجاح";
                return RedirectToAction(nameof(TermsAndConditions));
            }

            return View(model);
        }
    }
}

