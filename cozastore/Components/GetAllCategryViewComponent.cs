using AccountDAL.Eentiti.CozaStore;
using AccountDAL.Repositories;
using AutoMapper;
using BAL;
using cozastore.ViewModel.CozaMaster;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace cozastore.Components
{
    public class GetAllCategryViewComponent: ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllCategryViewComponent(IUnitOfWork unitOfWork ,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var rqf = HttpContext.Features.Get<IRequestCultureFeature>();
            var lang = rqf?.RequestCulture.UICulture.TwoLetterISOLanguageName ?? "en";
            var category = await _unitOfWork.Repository<Category>().GetAllAsync();
            var data = _mapper.Map<IEnumerable<CategoryViewModel>>(category, opts =>
            {
                opts.Items["lang"] = lang;
            });

            return View(data);
        }
    }
}
