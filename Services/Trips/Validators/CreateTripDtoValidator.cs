using drv_next_api.Services.Trips.Dto;
using FluentValidation;

namespace drv_next_api.Services.Trips.Validators
{
    public class CreateTripDtoValidator : AbstractValidator<CreateTripDto>
    {
        public CreateTripDtoValidator()
        {
            RuleFor(t => t.CustomerId).NotNull();
            RuleFor(t => t.Origin).NotNull().Length(1, 128);
            RuleFor(t => t.Destination).NotNull().Length(1, 128);
            RuleFor(t => t.Distance).NotNull().GreaterThan(0);
            RuleFor(t => t.Fare).NotNull().GreaterThan(0);
        }
    }
}