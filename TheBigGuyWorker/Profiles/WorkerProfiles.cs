using AutoMapper;
using TheTourGuy.Models.External;
using TheTourGuy.Models.Internal;

namespace TheBigGuyWorker.Profiles;

public class WorkerProfiles : Profile
{
    public WorkerProfiles()
    {
        CreateMap<TheBigGuyProductData, TheTourGuyModel>()
            .ForMember(dest => dest.DiscountPrice, opt => opt.MapFrom(o =>o.Price.Amount - (o.Price.Amount / (o.Price.Amount * 100))))
            .ForMember(dest => dest.MaximumGuests, opt => opt.MapFrom(o =>o.ProductDetailData.Capacity))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(o =>o.ProductDetailData.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(o =>o.ProductDetailData.ProductDescription))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(o => o.Pthotos.Select(pi => new ImageModel(){Url = pi}).ToList()))
            .ForMember(dest => dest.RegularPrice, opt => opt.MapFrom(o => o.Price.Amount));
    }
}