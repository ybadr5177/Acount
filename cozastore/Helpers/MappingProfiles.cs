using AccountDAL.Eentiti;
using AccountDAL.Eentiti.CozaStore;
using AccountDAL.Eentiti.Order_Aggregate;
using AccountDAL.Specifications.ProductsSpecification;
using AutoMapper;
using cozastore.Helpers.Resolver;
using cozastore.ViewModel;
using cozastore.ViewModel.Accessories;
using cozastore.ViewModel.CozaMaster;
using cozastore.ViewModel.Ordes;
using System.Collections.Generic;

namespace cozastore.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<SliderItem, SliderItemViewModel>()
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom<SliderImageResolver>())
            .ForMember(dest => dest.LinkUrl, opt => opt.MapFrom<SliderLinkResolver>())
            .ForMember(d => d.Title, o => o.MapFrom(
                (src, dest, destMember, ctx) =>
                    (string)ctx.Items["lang"] == "ar" ? src.Title_AR : src.Title
            ))
            .ForMember(d => d.Description, o => o.MapFrom(
                (src, dest, destMember, ctx) =>
                    (string)ctx.Items["lang"] == "ar" ? src.Description_AR : src.Description
            ));


            CreateMap<CustomerBasketViewModel, CustomerBasket>();
            CreateMap<BasketItemViewModel, BasketItem>();
            CreateMap<Category, CategoryViewModel>().ForMember(d => d.Picture, o => o.MapFrom<CategoryPictureUrlResolver>())
                .ForMember(n => n.Name_EN, o => o.MapFrom(
                (src, dest, destMember, ctx) =>
                    (string)ctx.Items["lang"] == "ar" ? src.Name_Ar : src.Name_EN
            )).ReverseMap();
            CreateMap<SubCategory, SubCategoryViewModel>()
               //.ForMember(d => d.Name_EN, o => o.MapFrom(s => s.Category.Name_EN))
               .ForMember(d => d.Picture, o => o.MapFrom<SubCategoryPictureUrlResolver>())
                .ForMember(d => d.Name_EN, o => o.MapFrom(
                (src, dest, destMember, ctx) =>
                    (string)ctx.Items["lang"] == "ar" ? src.Name_Ar : src.Name_EN
            ))
               .ReverseMap();
            CreateMap<Product, ProductDisplayViewModel>()
            .ForMember(d => d.Picture, o => o.MapFrom<ProductDisPlayPictureUrlResolver>())
            .ForMember(d => d.Address, o => o.MapFrom((src, dest, destMember, ctx) =>
                    (string)ctx.Items["lang"] == "ar" ? src.Address_Ar : src.Address_En
            ))
            .ForMember(d => d.description, o => o.MapFrom(
                (src, dest, destMember, ctx) =>
                    (string)ctx.Items["lang"] == "ar" ? src.description_Ar : src.description_En
            ))
            .ForMember(d => d.Price, o => o.MapFrom(p => p.BasePrice))
            .ForMember(d => d.ProductSize, o => o.MapFrom(p => p.ProductSizes))

           /* .ForMember(d => d.CategoryName, o => o.MapFrom(p => p.SubCategory.Name_EN))*/.ReverseMap();
            CreateMap<(ProductSpecParams param, List<ProductDisplayViewModel> PMap, IReadOnlyList<Category> CMap), ProductPageViewModel>()
             .ForMember(dest => dest.Filter, opt => opt.MapFrom(src => src.param))
             .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.PMap))
             .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.CMap));

            CreateMap<(ProductSpecParams param, List<ProductDisplayViewModel> PMap, List<SubCategoryViewModel> CMap), ProductAndSubCategoryPageViewModel>()
            .ForMember(dest => dest.Filter, opt => opt.MapFrom(src => src.param))
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.PMap))
            .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src => src.CMap));

            CreateMap<(ProductSpecParams param, List<ProductDisplayViewModel> PMap), ProductUsingSubCategoryPageViewModel>()
            .ForMember(dest => dest.Filter, opt => opt.MapFrom(src => src.param))
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.PMap));


            CreateMap<Product, DetailsProductDisplayViewModel>()
           .ForMember(d => d.Picture, o => o.MapFrom<DetailsProductDisplayPictureUrlResolver>())
            .ForMember(d => d.Address, o => o.MapFrom((src, dest, destMember, ctx) =>
                    (string)ctx.Items["lang"] == "ar" ? src.Address_Ar : src.Address_En
            ))
             .ForMember(d => d.description, o => o.MapFrom((src, dest, destMember, ctx) =>
                    (string)ctx.Items["lang"] == "ar" ? src.description_En : src.description_En
            ))
           .ForMember(d => d.ProductSize, o => o.MapFrom(p => p.ProductSizes));
            ;
            CreateMap<ProductSize, ProductSizeViewModel>()
            .ForMember(dest => dest.SizeLabel, opt => opt.MapFrom(src => src.Size.Label));

            CreateMap<FeedbackViewModel, Feedback>().ReverseMap();

            CreateMap<ContactUS, ContactUSdbViewModel>().ReverseMap();

            CreateMap<ProfileViewModel, ProfileForMapViewModel>().ForMember(d => d.ProfilePicture, o => o.MapFrom<ProfilePictureUrlResolver>()).ReverseMap();

            CreateMap<BasketItem, BasketItemViewModel>()
                // .ForMember(dest => dest.PictureUrl,
                //opt => opt.MapFrom<BasketPictureUrlResolver>())
                .ReverseMap();
            CreateMap<CustomerBasketViewModel, CustomerBasket>().ReverseMap();
            CreateMap<AddressViewModel, AccountDAL.Eentiti.Order_Aggregate.Address>().ReverseMap();

            CreateMap<Order, OrderToReturnViewModel>()
               .ForMember(d => d.DeliveryTime, O => O.MapFrom(S => S.DeliveryCost.DeliveryTime))
               .ForMember(d => d.DeliveryCost, O => O.MapFrom(S => S.DeliveryCost.Cost));

            CreateMap<OrderItem, OrderItemViewModel>()
                .ForMember(d => d.ProductId, O => O.MapFrom(S => S.ProductItemOrdered.ProductId))
                .ForMember(d => d.ProductName, O => O.MapFrom(S => S.ProductItemOrdered.ProductName));

            CreateMap<Favorites, FavoritesViewModel>();



        }
    }


}
