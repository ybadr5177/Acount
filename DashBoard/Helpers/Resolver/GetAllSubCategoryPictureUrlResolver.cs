using AccountDAL.Eentiti.CozaStore;
using AutoMapper;
using DashBoard.ViewModel.CozaMaster;

namespace DashBoard.Helpers.Resolver
{

    public class GetAllSubCategoryPictureUrlResolver : IValueResolver<SubCategory, GetAllSubCategoryViewModel, string>
    {
        public GetAllSubCategoryPictureUrlResolver(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        public string Resolve(SubCategory source, GetAllSubCategoryViewModel destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.Picture))
                return $"{Configuration["BaseApiUrl"]}files/image/{source.Picture}";
            return null;
        }
    }
}
