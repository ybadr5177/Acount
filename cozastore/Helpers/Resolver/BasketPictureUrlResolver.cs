using AccountDAL.Eentiti;
using AccountDAL.Eentiti.CozaStore;
using AutoMapper;
using cozastore.ViewModel;

namespace cozastore.Helpers.Resolver
{
    public class BasketPictureUrlResolver : IValueResolver<BasketItem, BasketItemViewModel, List<string>>
    {
        public BasketPictureUrlResolver(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        public List<string> Resolve(BasketItem source, BasketItemViewModel destination, List<string> destMember, ResolutionContext context)
        {
            if (source.PictureUrl != null && source.PictureUrl.Any())
                return source.PictureUrl
                 .Select(img => $"{Configuration["BaseApiUrl"]}files/image/{img}")
                 .ToList();
            return null;
        }
    }
}
