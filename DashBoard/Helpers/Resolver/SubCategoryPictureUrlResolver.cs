using AccountDAL.Eentiti.CozaStore;
using AutoMapper;
using DashBoard.ViewModel.CozaMaster;

namespace DashBoard.Helpers.Resolver
{
    public class SubCategoryPictureUrlResolver : IValueResolver<SubCategory, SubCategoryViewModel, string>
    {
        public SubCategoryPictureUrlResolver(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        public string Resolve(SubCategory source, SubCategoryViewModel destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.Picture))
                return $"{Configuration["BaseApiUrl"]}files/image/{source.Picture}";
            return null;
        }
    }
}
