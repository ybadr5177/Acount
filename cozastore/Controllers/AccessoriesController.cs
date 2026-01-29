using AccountDAL.Eentiti;
using AccountDAL.Eentiti.CozaStore;
using AccountDAL.Repositories;
using AutoMapper;
using cozastore.ViewModel.Accessories;
using cozastore.ViewModel.CozaMaster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace cozastore.Controllers
{
    public class AccessoriesController : BaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccessoriesController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Feedback()
        {
            var user = await _userManager.GetUserAsync(User);
            var model = new FeedbackViewModel
            {
                Email = user?.Email // نجيب البريد تلقائي
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Feedbacken(FeedbackViewModel feedbacks)
        {
            if (!ModelState.IsValid)
                return View(feedbacks);

            var MapFeedBack = _mapper.Map<FeedbackViewModel, Feedback>(feedbacks);
            var data = _unitOfWork.Repository<Feedback>().AddAsync(MapFeedBack);
            await _unitOfWork.Complete();

            TempData["Success"] = "Your complaint or suggestion has been submitted successfully!";
            return RedirectToAction("Feedback");
        }


        [HttpGet]
        public async Task<IActionResult> ContactUS()
        {
            var Data = await _unitOfWork.Repository<ContactUS>().GetFirstAsync();
            var DataMap = _mapper.Map<ContactUS, ContactUSdbViewModel>(Data);
            return View(DataMap);
        }


        [HttpGet]
        public async Task<IActionResult> TermsAndConditions()
        {
            var lastData = await _unitOfWork.Repository<TermsAndConditions>().GetLatestAsync(x => x.LastUpdated);
            return View(lastData);
        }

    }
}
