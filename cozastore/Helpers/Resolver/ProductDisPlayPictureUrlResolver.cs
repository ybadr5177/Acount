using AccountDAL.Eentiti.CozaStore;
using AutoMapper;
using cozastore.ViewModel.CozaMaster;

namespace cozastore.Helpers.Resolver
{
    public class ProductDisPlayPictureUrlResolver : IValueResolver<Product, ProductDisplayViewModel, List<string>>
    {
        public IConfiguration Configuration { get; }
        public ProductDisPlayPictureUrlResolver(IConfiguration configuration)
        {
            Configuration = configuration;

        }
        //public string Resolve(Product source, ProductDisplayViewModel destination, string destMember, ResolutionContext context)
        //{
        //    if (!string.IsNullOrEmpty(source.Address_En))
        //        return $"{Configuration["BaseApiUrl"]}files/image/{source.ImageName}";
        //    return null;
        //}

        public List<string> Resolve(Product source, ProductDisplayViewModel destination, List<string> destMember, ResolutionContext context)
        {
            if (source.ImageName != null && source.ImageName.Any())
                return source.ImageName
                 .Select(img => $"{Configuration["BaseApiUrl"]}files/image/{img.ImageUrl}")
                 .ToList();
            return null;
        }
    }
}

