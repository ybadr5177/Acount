using AccountDAL.Eentiti.CozaStore;
using AccountDAL.Repositories;
using AutoMapper;
using cozastore.ViewModel;
using cozastore.ViewModel.Accessories;
using cozastore.ViewModel.CozaMaster;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace cozastore.Components
{
    public class SocialLinksViewComponent: ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IMapper _mapper;
        public SocialLinksViewComponent(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var Data = await _unitOfWork.Repository<ContactUS>().GetFirstAsync();
            var DataMap = _mapper.Map<ContactUS, ContactUSdbViewModel>(Data);
            var rqf = HttpContext.Features.Get<IRequestCultureFeature>();
            var lang = rqf?.RequestCulture.UICulture.TwoLetterISOLanguageName ?? "en";
            var category = await _unitOfWork.Repository<Category>().GetAllAsync();
            var data = _mapper.Map<IEnumerable<CategoryViewModel>>(category, opts =>
            {
                opts.Items["lang"] = lang;
            });

            var NewData = new SocialLinksAndCatagreyViewModel
            {
                contactUSdb = DataMap,
                Category = data

            };
            return View(NewData);
        }
    }
}
