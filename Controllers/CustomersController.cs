using System.Linq;
using System.Threading.Tasks;
using drv_next_api.Controllers.Models;
using drv_next_api.Data;
using drv_next_api.Models;
using drv_next_api.Services.Customers;
using drv_next_api.Services.Customers.Dto;
using drv_next_api.Services.Customers.Exceptions;
using drv_next_api.Services.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace drv_next_api.Controllers
{
    [ApiController]
    [Route("customers")]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationContext _appCtx;
        private readonly ILogger<CustomerController> _logger;
        private readonly CustomersService _service;

        public CustomerController(ILogger<CustomerController> logger, ApplicationContext appCtx,
            CustomersService service)
        {
            _logger = logger;
            _appCtx = appCtx;
            _service = service;
        }

        [HttpGet("verify-phone-number")]
        public async Task<ActionResult> Get([FromQuery(Name = "q")] string q = "")
        {
            return await _service.VerifyPhoneNumber(q) ? new ConflictResult() : new OkResult();
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<Customer>>> Get(
            [FromQuery(Name = "skip")] int skip = 0,
            [FromQuery(Name = "q")] string q = "",
            [FromQuery(Name = "take")] int take = 15)
        {
            var queryable = _appCtx.Customers.AsQueryable();
            if (q != null && q.Trim() != "") queryable = queryable.Where(c => c.Name.Contains(q.Trim()));

            return new OkObjectResult(new PagedResult<Customer>(
                take,
                skip,
                await queryable.OrderBy(k => k.Name).Skip(skip).Take(take).ToListAsync()
            ));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Customer>> Get([FromRoute(Name = "id")] int id)
        {
            var customer = await _appCtx.Customers.Where(c => c.Id == id).FirstOrDefaultAsync();
            return customer != null ? new OkObjectResult(customer) : new NotFoundResult();
        }


        [HttpPost]
        public async Task<ActionResult<Customer>> Post(CreateCustomerDto customerData)
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
        [Route("{customerId:int}")]
        public async Task<ActionResult> Delete([FromRoute(Name = "customerId")] int customerId)
        {
            try
            {
                await _service.DeleteCustomer(new DeleteCustomerDto {CustomerId = customerId});
                return new OkResult();
            }
            catch (CustomerNotFoundException)
            {
                return new NotFoundResult();
            }
        }

        [HttpPatch]
        [Route("{customerId}")]
        public async Task<ActionResult> UpdateData([FromRoute(Name = "customerId")] int customerId,
            UpdateCustomerDataDto dto)
        {
            try
            {
                await _service.UpdateCustomerData(new UpdateCustomerDataDto {CustomerId = customerId, Name = dto.Name});
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