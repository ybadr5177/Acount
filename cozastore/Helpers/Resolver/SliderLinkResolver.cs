using AccountDAL.Eentiti.CozaStore;
using AutoMapper;
using cozastore.ViewModel;

namespace cozastore.Helpers.Resolver
{
    public class SliderLinkResolver : IValueResolver<SliderItem, SliderItemViewModel, string?>
    {
        private readonly IConfiguration Configuration;

        public SliderLinkResolver(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public string? Resolve(SliderItem source, SliderItemViewModel destination, string? destMember, ResolutionContext context)
        {
            return source.Type.ToLower() switch
            {
                "product" when source.ProductId.HasValue => $"{Configuration["https://cozastore.runasp.net/"]}Mastar/DetailsProduct/{source.ProductId.Value}",
                "link" => source.ExternalLink,
                _ => null
            };
        }
    }
}
