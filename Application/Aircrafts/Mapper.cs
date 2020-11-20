using Application.Aircrafts.Dto;
using AutoMapper;
using Domain;
using System.Linq;

namespace Application.Aircrafts
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<Image, ImageDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.ImageUrl, o => o.MapFrom(s => s.ImageUrl));
            CreateMap<Aircraft, AircraftDto>()
                .ForMember(d => d.AircraftName, o => o.MapFrom(s => s.AircraftName))
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.Description, o => o.MapFrom(s => s.Description))
                .ForMember(d => d.Categories, o => o.MapFrom(s => s.AircraftCategory.Select(el => el.Category.CategoryName)))
                .ForMember(d => d.Types, o => o.MapFrom(s => s.AircraftTypes.Select(el => el.Type.TypeName)))
                .ForMember(d => d.Images, o => o.MapFrom(s => s.Images))
                .ForMember(d => d.YearInService, o => o.MapFrom(s => s.YearInService))
                .ForMember(d => d.Country, o => o.MapFrom(s => s.Country))
                .ForMember(d => d.Comments, o => o.MapFrom(s => s.Comments));
        }
    }
}
