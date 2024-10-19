using AutoMapper;
using TheTourGuy.Models.External;
using TheTourGuy.Models.Internal;

namespace SomeOtherGuyWorker.Profiles;

public class WorkerProfiles : Profile
{
    public WorkerProfiles()
    {
        CreateMap<SomeOtherGuyModel, TheTourGuyModel>()
            .ForMember(dest => dest.DiscountPrice, opt => opt.MapFrom(o =>o.Price - (o.Price / o.DiscountPercentage)))
            .ForMember(dest => dest.MaximumGuests, opt => opt.MapFrom(o =>o.Capacity))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(o =>o.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(o =>o.ProductDescription))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(o => o.ImageUrls.Select(pi => new ImageModel(){Url = pi}).ToList()))
            .ForMember(dest => dest.RegularPrice, opt => opt.MapFrom(o => o.Price));
        
    }
}