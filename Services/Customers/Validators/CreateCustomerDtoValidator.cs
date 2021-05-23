using FluentValidation;
using drv_next_api.Services.Customers.Dto;

namespace drv_next_api.Services.Customers.Validators
{
    public class CreateCustomerDtoValidator : AbstractValidator<CreateCustomerDto>
    {

        public CreateCustomerDtoValidator()
        {
            RuleFor(c => c.Name)
                .NotNull().WithMessage("CUSTOMER_NAME_NULL")
                .Length(1, 128).WithMessage("CUSTOMER_NAME_LENGTH");
            RuleFor(c => c.PhoneNumber)
                .NotNull().WithMessage("CUSTOMER_PHONENUMBER_NULL")
                .Matches("^(\\+55 \\([1-9][1-9]\\) 9?[1-9][0-9][0-9][0-9]-[0-9][0-9][0-9][0-9])$").WithMessage("CUSTOMER_PHONENUMBER_MATCH");
        }
    }
}