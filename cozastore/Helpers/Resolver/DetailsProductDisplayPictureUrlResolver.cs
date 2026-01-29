using AccountDAL.Eentiti.CozaStore;
using AutoMapper;
using cozastore.ViewModel.CozaMaster;

namespace cozastore.Helpers.Resolver
{
    public class DetailsProductDisplayPictureUrlResolver: IValueResolver<Product, DetailsProductDisplayViewModel, List<string>>
    {
        public IConfiguration Configuration { get; }
        public DetailsProductDisplayPictureUrlResolver(IConfiguration configuration)
        {
            Configuration = configuration;


        }

        //public string Resolve(Product source, DetailsProductDisplayViewModel destination, string destMember, ResolutionContext context)
        //{
            
        //    if (!string.IsNullOrEmpty(source.Address_En))
        //        return $"{Configuration["BaseApiUrl"]}files/image/{source.ImageName}";
        //    return null;
        //}

        public List<string> Resolve(Product source, DetailsProductDisplayViewModel destination, List<string> destMember, ResolutionContext context)
        {
            if (source.ImageName != null && source.ImageName.Any())
                return source.ImageName
                 .Select(img => $"{Configuration["BaseApiUrl"]}files/image/{img.ImageUrl}")
                 .ToList();
            return null;
        }
    }
}
