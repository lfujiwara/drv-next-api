using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using drv_next_api.Models;
using drv_next_api.Data;
using drv_next_api.Controllers.Models;
using drv_next_api.Services.Customers;
using drv_next_api.Services.Customers.Dto;
using drv_next_api.Services.Customers.Exceptions;
using drv_next_api.Services.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace drv_next_api.Controllers
{
    [ApiController]
    [Route("customers")]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly CustomersService _service;
        private readonly ApplicationContext _appCtx;

        public CustomerController(ILogger<CustomerController> logger, ApplicationContext appCtx, CustomersService service)
        {
            _logger = logger;
            _appCtx = appCtx;
            _service = service;
        }

        [HttpGet]
        public ActionResult<PagedResult<Customer>> Get([FromQuery(Name = "skip")] int skip = 0, [FromQuery(Name = "take")] int take = 15)
        {
            return new OkObjectResult(new PagedResult<Customer>(take, skip, _appCtx.Customers.OrderBy(k => k.Name).Skip(skip).Take(take).ToList()));
        }


        [HttpPost]
        public async Task<ActionResult<IEnumerable<Customer>>> Post(CreateCustomerDto customerData)
        {
            try
            {
                var customer = await _service.CreateCustomer(customerData);
                return new OkObjectResult(customer);
            }
            catch (ServiceValidationException ex)
            {
                return new BadRequestObjectResult(ex.result);
            }
            catch (CustomerDuplicatePhoneNumberException)
            {
                return new ConflictResult();
            }
        }

        [HttpDelete]
        [Route("{customerId}")]
        public async Task<ActionResult> Delete([FromRoute(Name = "customerId")] int customerId)
        {
            try
            {
                await _service.DeleteCustomer(new DeleteCustomerDto { CustomerId = customerId });
                return new OkResult();
            }
            catch (CustomerNotFoundException)
            {
                return new NotFoundResult();
            }
        }

        [HttpPatch]
        [Route("{customerId}")]
        public async Task<ActionResult> UpdateData([FromRoute(Name = "customerId")] int customerId, UpdateCustomerDataDto dto)
        {
            try
            {
                await _service.UpdateCustomerData(new UpdateCustomerDataDto { CustomerId = customerId, Name = dto.Name });
                return new OkResult();
            }
            catch (ServiceValidationException ex)
            {
                return new BadRequestObjectResult(ex.result);
            }
            catch (CustomerNotFoundException)
            {
                return new NotFoundResult();
            }
        }
    }
}
