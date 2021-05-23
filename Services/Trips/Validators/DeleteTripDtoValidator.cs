using drv_next_api.Services.Trips.Dto;
using FluentValidation;

namespace drv_next_api.Services.Trips.Validators
{
    public class DeleteTripDtoValidator : AbstractValidator<DeleteTripDto>
    {
        public DeleteTripDtoValidator()
        {
            RuleFor(t => t.TripId).NotNull();
        }
    }
}