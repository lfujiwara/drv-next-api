using FluentValidation;
using drv_next_api.Services.Customers.Dto;

namespace drv_next_api.Services.Customers.Validators
{
    public class DeleteCustomerDtoValidator : AbstractValidator<DeleteCustomerDto>
    {

        public DeleteCustomerDtoValidator()
        {
            RuleFor(c => c.CustomerId).NotNull();
        }
    }
}