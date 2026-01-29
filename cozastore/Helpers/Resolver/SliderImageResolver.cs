using AccountDAL.Eentiti.CozaStore;
using AccountDAL.Repositories;
using AutoMapper;
using cozastore.ViewModel;

namespace cozastore.Helpers.Resolver
{
    public class SliderImageResolver : IValueResolver<SliderItem, SliderItemViewModel, string>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration Configuration;

        public SliderImageResolver(IUnitOfWork unitOfWork,IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            Configuration = configuration;
        }
        public string Resolve(SliderItem source, SliderItemViewModel destination, string destMember, ResolutionContext context)
        {
            //if (source.Type.ToLower() == "product" && source.ProductId.HasValue)
            //{
            //    var product = _unitOfWork.Repository<Product>().GetByIdAsync(source.ProductId.Value).Result;
            //    return $"{Configuration["BaseApiUrl"]}{product?.ImageName}" ?? ""; 
            //}

            //return $"{Configuration["BaseApiUrl"]} {source.ImagePath}"?? "";

            if (!string.IsNullOrEmpty(source.ImagePath))
                return $"{Configuration["BaseApiUrl"]}files/image/{source.ImagePath}";
            return null;
        }
    }
}
