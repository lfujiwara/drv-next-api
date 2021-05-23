using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using drv_next_api.Data;
using drv_next_api.Models;
using drv_next_api.Services.Customers.Exceptions;
using drv_next_api.Services.Exceptions;
using drv_next_api.Services.Trips.Dto;
using drv_next_api.Services.Trips.Exceptions;
using drv_next_api.Services.Trips.Validators;
using Microsoft.EntityFrameworkCore;

namespace drv_next_api.Services.Trips
{
    public class TripsService
    {
        private readonly ApplicationContext _ctx;
        private readonly IMapper _mapper;

        private readonly CreateTripDtoValidator createTripDtoValidator = new CreateTripDtoValidator();
        private readonly DeleteTripDtoValidator deleteTripDtoValidator = new DeleteTripDtoValidator();

        public TripsService(ApplicationContext ctx, IMapper mapper)
        {
            _ctx = ctx;
            _mapper = mapper;
        }


        /// <exception cref="ServiceValidationException"/>
        public async Task<Trip> CreateTrip(CreateTripDto dto)
        {
            var vResult = await createTripDtoValidator.ValidateAsync(dto);
            if (!vResult.IsValid)
                throw new ServiceValidationException(vResult);

            if (await _ctx.Customers.Where(p => p.Id == dto.CustomerId).Select(p => p.Id).CountAsync() == 0) throw new CustomerNotFoundException();

            var trip = _mapper.Map<CreateTripDto, Trip>(dto);
            var saveResult = await _ctx.Trips.AddAsync(trip);
            await _ctx.SaveChangesAsync();

            return saveResult.Entity;
        }

        /// <exception cref="ServiceValidationException"/>
        /// <exception cref="TripNotFoundException"/>
        public async Task DeleteTrip(DeleteTripDto dto)
        {
            var vResult = await deleteTripDtoValidator.ValidateAsync(dto);
            if (!vResult.IsValid)
                throw new ServiceValidationException(vResult);

            var entity = await _ctx.Trips.FindAsync(new object[] { dto.TripId });
            if (entity == null) throw new TripNotFoundException();

            var result = _ctx.Remove(entity);
            await _ctx.SaveChangesAsync();
        }
    }
}