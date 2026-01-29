using AccountDAL.Eentiti.CozaStore;
using AutoMapper;
using DashBoard.ViewModel.CozaMaster;

namespace DashBoard.Helpers.Resolver
{
    public class CategoryPictureUrlResolver: IValueResolver<Category, CategoryViewModel, string>
    {
        public CategoryPictureUrlResolver(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public string Resolve(Category source, CategoryViewModel destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.Picture))
                return $"{Configuration["BaseApiUrl"]}files/image/{source.Picture}";
            return null;
        }

    }
}
