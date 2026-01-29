using AccountDAL.Eentiti.CozaStore;
using AutoMapper;
using DashBoard.ViewModel.CozaMaster;

namespace DashBoard.Helpers.Resolver
{
    public class ProductPictureUrlResolver: IValueResolver<Product, productDisplayViewModel, List<string>>
    {
        public ProductPictureUrlResolver(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        //public string Resolve(Product source, productDisplayViewModel destination, string destMember, ResolutionContext context)
        //{
        //    if (!string.IsNullOrEmpty(source.Address_En))
        //        return $"{Configuration["BaseApiUrl"]}files/image/{source.ImageName}";
        //    return null;
        //}

        public List<string> Resolve(Product source, productDisplayViewModel destination, List<string> destMember, ResolutionContext context)
        {
            if (source.ImageName != null && source.ImageName.Any())
                return source.ImageName
                 .Select(img => $"{Configuration["BaseApiUrl"]}files/image/{img.ImageUrl}")
                 .ToList();
            return null;
        }
    }
}
