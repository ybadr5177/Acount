using AccountDAL.Eentiti.CozaStore;
using AutoMapper;
using DashBoard.ViewModel.CozaMaster;

namespace DashBoard.Helpers.Resolver
{
    public class GetAllPredectForSliderImageUrlResolver : IValueResolver<Product, GetAllPredectForSlider, List<string>>
    {
        public GetAllPredectForSliderImageUrlResolver(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        //public string Resolve(Product source, GetAllPredectForSlider destination, string destMember, ResolutionContext context)
        //{
        //    if (!string.IsNullOrEmpty(source.Address_Ar))
        //        return $"{Configuration["BaseApiUrl"]}files/image/{source.ImageName}";
        //    return null;
        //}

        public List<string> Resolve(Product source, GetAllPredectForSlider destination, List<string> destMember, ResolutionContext context)
        {
            if (source.ImageName != null && source.ImageName.Any())
                return source.ImageName
                 .Select(img => $"{Configuration["BaseApiUrl"]}files/image/{img.ImageUrl}")
                 .ToList();
            return null;
        }
    }
}
