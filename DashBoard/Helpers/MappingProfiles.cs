using AccountDAL.Eentiti;
using AccountDAL.Eentiti.CozaStore;
using AutoMapper;
using DashBoard.Helpers.Resolver;
using DashBoard.ViewModel;
using DashBoard.ViewModel.Accessories;
using DashBoard.ViewModel.CozaMaster;
using System.Drawing;

namespace DashBoard.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Category, CategoryViewModel>().ForMember(d => d.Picture, o => o.MapFrom<CategoryPictureUrlResolver>()).ReverseMap();
            CreateMap<SubCategory, SubCategoryViewModel>()
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name_EN))
                .ForMember(d => d.Picture, o => o.MapFrom<SubCategoryPictureUrlResolver>()).ReverseMap()
                  .ForMember(dest => dest.Category, opt => opt.Ignore());
            CreateMap<SubCategory, GetAllSubCategoryViewModel>()
               .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name_EN))
               .ForMember(d => d.Picture, o => o.MapFrom<GetAllSubCategoryPictureUrlResolver>()).ReverseMap();
            CreateMap<SubCategory, EditAndCreatSubCategoryViewModel>().ReverseMap();
            //.ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.Name_EN))
            /*.ForMember(d => d.Picture, o => o.MapFrom<GetAllSubCategoryPictureUrlResolver>()).ReverseMap()*/
            ;
            CreateMap<DifferentSize, DifferentSizeViewModel>()
           //    .ForMember(dest => dest.Label, opt => opt.MapFrom(src =>
           // src.Type == SizeType.Dimension && src.Dimensions != null
           //? $"{src.Dimensions.Width} x {src.Dimensions.Height} x {src.Dimensions.Depth}"
           //: src.Label))
           /* .ForMember(dest => dest.Dimensions, opt => opt.MapFrom(src => src.Dimensions))*/;
            CreateMap<DifferentSizeViewModel, DifferentSize>()
            /*.ForMember(dest => dest.Dimensions, opt => opt.MapFrom(src => src.Dimensions))*/;
            CreateMap<DimensionSizeViewModel, DimensionSize>().ReverseMap();
            CreateMap<DiscountCodesViewModel, opponent>().ReverseMap();





            //    .ForMember(dest => dest.SizeLabel, opt => opt.MapFrom(src => src.Size.Type == SizeType.Dimension && src.Size.Dimensions != null
            //? $"{src.Size.Dimensions.Width} x {src.Size.Dimensions.Height} x {src.Size.Dimensions.Depth}"
            //: src.Size.Label));







            CreateMap<SliderViewModel, SliderItem>();


            CreateMap<NameSizeViewModel, SizeName>().ReverseMap();

            CreateMap<AddProductImagesViewModel, ProductImage>().ReverseMap();
            CreateMap<Feedback, FeedBackViewModel>().ReverseMap();
            CreateMap<ContactUS, ContactUSViewModel>().ReverseMap();

            CreateMap<ProductViewModel, Product>()
             .ForMember(dp => dp.BaseDiscountPrice, opt => opt.MapFrom(src =>
             src.BaseDiscountPrice
                  // لو DiscountPrice = null وجالك Discountrate → احسب من BasePrice
                  ?? (src.Discountrate.HasValue
                      ? (src.BasePrice == 0
                                           ? 0m
                         : src.BasePrice - (src.BasePrice * src.Discountrate.Value / 100m))
                         // لو الاتنين null → خليه null
                         : (decimal?)null)
           ))
           .ForMember(dest => dest.ProductSizes, opt => opt.MapFrom(src => src.ProductSizes)).ReverseMap();

            CreateMap<ProductSize, ProductSizeEntryViewModel>();

            CreateMap<Product, GetAllPredectForSlider>()
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Address_En))
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.price, opt => opt.MapFrom(src => src.BasePrice))
        .ForMember(d => d.Imega, o => o.MapFrom<GetAllPredectForSliderImageUrlResolver>())
         ;



            CreateMap<ProductImage, ProductImagesViewModel>()
               .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom<GetAllPredectImageUrlResolver>());



            CreateMap<Product, productDisplayViewModel>()
         .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Address_En))
         .ForMember(dest => dest.description, opt => opt.MapFrom(src => src.description_En))
         .ForMember(dest => dest.SubCategoryName, opt => opt.MapFrom(src => src.SubCategory.Name_EN))
          .ForMember(dest => dest.FirstImage,
              opt => opt.MapFrom(src => src.ImageName.FirstOrDefault().ImageUrl))
         .ForMember(d => d.Picture, o => o.MapFrom<ProductPictureUrlResolver>());



            CreateMap<ProductSizeEntryViewModel, ProductSize>()
          .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
           .ForMember(dp => dp.DiscountPrice, opt => opt.MapFrom(src =>
          src.DiscountPrice
                  // لو DiscountPrice = null وجالك Discountrate → احسب من BasePrice
                  ?? (src.Discountrate.HasValue
                      ? (src.Price == 0
                                           ? 0m
                         : src.Price - (src.Price * src.Discountrate.Value / 100m))
                         // لو الاتنين null → خليه null
                         : (decimal?)null)
           ));
            CreateMap<DeliveryCountryViewModel,DeliveryCountry>().ReverseMap();

            CreateMap<DeliveryCostViewModel, DeliveryCost>().ReverseMap();

            CreateMap<DeliveryCost, DisplayDeliveryCostViewModel>()
               .ForMember(d => d.DeliveryCountry, o => o.MapFrom(s => s.DeliveryCountry.Name_EN));
        }
    }
}
