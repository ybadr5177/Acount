using AccountDAL.Eentiti.CozaStore;
using AccountDAL.Repositories;
using AccountDAL.Specifications.ProductsSpecification;
using AutoMapper;
using cozastore.ViewModel;
using cozastore.ViewModel.CozaMaster;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace cozastore.Components
{
    public class GetAllProductViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetAllProductViewComponent(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var rqf = HttpContext.Features.Get<IRequestCultureFeature>();
            var lang = rqf?.RequestCulture.UICulture.TwoLetterISOLanguageName ?? "en";

            var sliders = await _unitOfWork.Repository<SliderItem>().GetAllAsync();
            var viewModels = _mapper.Map<List<SliderItemViewModel>>(sliders, opt => { opt.Items["lang"] = lang; });
            return View(viewModels);
           
        }
    }
}
