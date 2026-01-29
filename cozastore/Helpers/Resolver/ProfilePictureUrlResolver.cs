using AccountDAL.Eentiti.CozaStore;
using AutoMapper;
using cozastore.ViewModel;
using cozastore.ViewModel.CozaMaster;

namespace cozastore.Helpers.Resolver
{
    public class ProfilePictureUrlResolver : IValueResolver<ProfileViewModel, ProfileForMapViewModel, string>
    {
        public ProfilePictureUrlResolver(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        public string Resolve(ProfileViewModel source, ProfileForMapViewModel destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.ProfilePicture))
                return $"{Configuration["BaseApiUrlAcount"]}files/image/{source.ProfilePicture}";
            return null;
        }
    }
}
