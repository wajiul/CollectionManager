using AutoMapper;
using CollectionManager.Models;
using CollectionManager.Data_Access.Entities;
using Newtonsoft.Json;
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

            CreateMap<ItemModel, Item>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.CollectionId, opt => opt.MapFrom(src => src.CollectionId))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src =>
                    JsonConvert.DeserializeObject<List<TagValue>>(src.Tags).Select(t => new Tag
                    {
                        Name = t.Value
                    }).ToList()
                ))
                .ForMember(dest => dest.FieldValues, opt => opt.MapFrom(src =>
                    src.FieldValues.Select(c => new CustomFieldValue
                    {
                        ItemId = c.ItemId,
                        Value = c.Value,
                        CustomFieldId = c.Id
                    }).ToList()
                ));


            CreateMap<Item, ItemModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => JsonConvert.SerializeObject(
                    src.Tags.Select(t => t.Name).ToList()
                   )
                ))
                .ForMember(dest => dest.FieldValues, opt => opt.MapFrom(src => 
                    src.FieldValues.Select(f => new CustomFieldValueModel
                    {
                        Id = f.Id,
                        Type = f.CustomField.Type,
                        Name = f.CustomField.Name,
                        Value = f.Value
                    }).ToList()
                ));


            CreateMap<LikeModel, Like>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Item, opt => opt.Ignore());


            CreateMap<CommentModel, Comment>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Item, opt => opt.Ignore());


        }
    }
}
