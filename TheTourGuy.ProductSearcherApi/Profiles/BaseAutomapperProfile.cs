using AutoMapper;
using TheTourGuy.DTO.Request;
using TheTourGuy.Models.Internal;

namespace ProductSearcherApi.Profiles;

public class BaseAutomapperProfile: Profile
{
    public BaseAutomapperProfile()
    {
        CreateMap<ProductFilterRequest, ProductFilter>();
    }
}