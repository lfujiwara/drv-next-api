using System;
using System.Threading.Tasks;
using drv_next_api.Models;
using drv_next_api.Data;
using drv_next_api.Services.Customers.Dto;
using drv_next_api.Services.Customers.Exceptions;
using drv_next_api.Services.Customers.Validators;
using drv_next_api.Services.Exceptions;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace drv_next_api.Services.Customers
{
    public class CustomersService
    {
        private readonly ApplicationContext _ctx;
        private readonly CreateCustomerDtoValidator createCustomerDtoValidator = new CreateCustomerDtoValidator();
        private readonly DeleteCustomerDtoValidator deleteCustomerDtoValidator = new DeleteCustomerDtoValidator();
        private readonly UpdateCustomerDataDtoValidator updateCustomerDataDtoValidator = new UpdateCustomerDataDtoValidator();

        public CustomersService(ApplicationContext ctx)
        {
            _ctx = ctx;
        }


        /// <exception cref="ServiceValidationException"></exception>
        /// <exception cref="CustomerDuplicatePhoneNumberException"></exception>
        public async Task<Customer> CreateCustomer(CreateCustomerDto dto)
        {
            var vResult = await createCustomerDtoValidator.ValidateAsync(dto);
            if (!vResult.IsValid)
                throw new ServiceValidationException(vResult);
            if (await _ctx.Customers.Select(p => p.PhoneNumber).Where(phoneNumber => phoneNumber == dto.PhoneNumber).CountAsync() > 0)
                throw new CustomerDuplicatePhoneNumberException();


            var customer = new Customer();
            customer.Name = dto.Name;
            customer.PhoneNumber = dto.PhoneNumber;

            var result = await _ctx.Customers.AddAsync(customer);
            await _ctx.SaveChangesAsync();

            return result.Entity;
        }

        /// <exception cref="ServiceValidationException"></exception>
        /// <exception cref="CustomerNotFoundException"></exception>
        public async Task DeleteCustomer(DeleteCustomerDto dto)
        {
            var vResult = await deleteCustomerDtoValidator.ValidateAsync(dto);
            if (!vResult.IsValid)
                throw new ServiceValidationException(vResult);

            var entity = await _ctx.Customers.FindAsync(new object[] { dto.CustomerId });
            if (entity == null) throw new CustomerNotFoundException();

            var result = _ctx.Remove(entity);
            await _ctx.SaveChangesAsync();
        }

        /// <exception cref="ServiceValidationException"></exception>
        /// <exception cref="CustomerNotFoundException"></exception>
        /// <exception cref="CustomerDuplicatePhoneNumberException"></exception>
        public async Task UpdateCustomerData(UpdateCustomerDataDto dto)
        {
            var vResult = await updateCustomerDataDtoValidator.ValidateAsync(dto);
            if (!vResult.IsValid)
                throw new ServiceValidationException(vResult);

            var entity = await _ctx.Customers.FindAsync(new object[] { dto.CustomerId });
            if (entity == null) throw new CustomerNotFoundException();

            if (dto.Name != null) entity.Name = dto.Name;
            if (dto.PhoneNumber != null && dto.PhoneNumber != entity.PhoneNumber)
            {
                if (await _ctx.Customers.Where(c => c.PhoneNumber == dto.PhoneNumber).AnyAsync())
                    throw new CustomerDuplicatePhoneNumberException();
                entity.PhoneNumber = dto.PhoneNumber;
            }

            await _ctx.SaveChangesAsync();
        }

        public Task<bool> VerifyPhoneNumber(string phoneNumber)
        {
            return _ctx.Customers.Where(c => c.PhoneNumber == phoneNumber).AnyAsync();
        }
    }
}