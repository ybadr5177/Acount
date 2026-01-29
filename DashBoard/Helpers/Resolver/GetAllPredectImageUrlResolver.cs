using AccountDAL.Eentiti.CozaStore;
using AutoMapper;
using DashBoard.ViewModel;

namespace DashBoard.Helpers.Resolver
{
    public class GetAllPredectImageUrlResolver : IValueResolver<ProductImage, ProductImagesViewModel, string>
    {
        public IConfiguration Configuration { get; }
        public GetAllPredectImageUrlResolver(IConfiguration configuration)
        {
            Configuration = configuration;


        }
        public string Resolve(ProductImage source, ProductImagesViewModel destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.ImageUrl))
                return $"{Configuration["BaseApiUrl"]}files/image/{source.ImageUrl}";
            return null;
        }
    }
}
