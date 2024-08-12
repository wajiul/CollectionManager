using AutoMapper;
using CollectionManager.Models;
using CollectionManager.Data_Access.Entities;
namespace CollectionManager.Mapping
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<CollectionModel, Collection>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.CustomFields, opt => opt.Ignore())
                .ForMember(dest => dest.Items, opt => opt.Ignore());

            CreateMap<Collection, CollectionModel>();
        }
    }
}
