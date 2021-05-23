using FluentValidation;
using drv_next_api.Services.Customers.Dto;

namespace drv_next_api.Services.Customers.Validators
{
    public class UpdateCustomerDataDtoValidator : AbstractValidator<UpdateCustomerDataDto>
    {

        public UpdateCustomerDataDtoValidator()
        {
            RuleFor(c => c.Name)
                .Length(1, 128).WithMessage("CUSTOMER_NAME_LENGTH");
        }
    }
}