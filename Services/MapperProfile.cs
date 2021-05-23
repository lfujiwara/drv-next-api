using AutoMapper;
using drv_next_api.Models;
using drv_next_api.Services.Trips.Dto;

namespace drv_next_api.Services
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CreateTripDto, Trip>();
        }
    }
}