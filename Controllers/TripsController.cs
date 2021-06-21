using System;
using System.Linq;
using System.Threading.Tasks;
using drv_next_api.Controllers.Models;
using drv_next_api.Data;
using drv_next_api.Models;
using drv_next_api.QueryServices.Trips;
using drv_next_api.QueryServices.Trips.Dto;
using drv_next_api.Services.Customers.Exceptions;
using drv_next_api.Services.Exceptions;
using drv_next_api.Services.Trips;
using drv_next_api.Services.Trips.Dto;
using drv_next_api.Services.Trips.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace drv_next_api.Controllers
{
    [ApiController]
    [Route("trips")]
    public class TripsController : ControllerBase
    {
        private readonly ApplicationContext _appCtx;
        private readonly ILogger<TripsController> _logger;
        private readonly TripsService _service;
        private readonly TripsQueryService _tripsQueryService;

        public TripsController(ILogger<TripsController> logger, ApplicationContext appCtx, TripsService service,
            TripsQueryService tripsQueryService)
        {
            _logger = logger;
            _appCtx = appCtx;
            _service = service;
            _tripsQueryService = tripsQueryService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<Trip>>> Get(
            [FromQuery(Name = "skip")] int skip = 0,
            [FromQuery(Name = "take")] int take = 15
        )
        {
            DateTime? from = GetDateFromQuery("from"), to = GetDateFromQuery("to");

            return new OkObjectResult(
                new PagedResult<Trip>(
                    take,
                    skip,
                    await _tripsQueryService.GetTripsWithinDateRange(from, to)
                        .OrderBy(k => k.CustomerId)
                        .Skip(skip)
                        .Take(take)
                        .Include(t => t.Customer)
                        .ToListAsync()
                )
            );
        }

        [HttpGet("summary")]
        public async Task<ActionResult<TripSummary>> GetSummary()
        {
            DateTime? from = GetDateFromQuery("from"), to = GetDateFromQuery("to");

            return new OkObjectResult(await _tripsQueryService.GetSummary(from, to));
        }
        
        [HttpGet("multi-summary")]
        public async Task<ActionResult<TripSummary>> GetMultiSummary()
        {
            DateTime? from = GetDateFromQuery("from"), to = GetDateFromQuery("to");

            return new OkObjectResult(await _tripsQueryService.GetMultiTripSummary(from, to));
        }


        [HttpGet("customer/{customerId:int}")]
        public async Task<ActionResult<PagedResult<Trip>>> GetFromCustomer(
            [FromRoute(Name = "customerId")] int customerId,
            [FromQuery(Name = "skip")] int skip = 0,
            [FromQuery(Name = "take")] int take = 15
        )
        {
            if (!await _appCtx.Customers.Where(c => c.Id == customerId).AnyAsync()) return new NotFoundResult();

            DateTime? from = GetDateFromQuery("from"), to = GetDateFromQuery("to");

            var trips = await _tripsQueryService.GetTripsFromCustomer(customerId, from, to).Skip(skip)
                .Take(take)
                .ToListAsync();

            return new OkObjectResult(new PagedResult<Trip>(take, skip, trips));
        }

        [HttpGet]
        [Route("customer/{customerId:int}/summary")]
        public async Task<ActionResult> GetSummaryFromCustomer(
            [FromRoute(Name = "customerId")] int customerId,
            [FromQuery(Name = "skip")] int skip = 0,
            [FromQuery(Name = "take")] int take = 15
        )
        {
            if (!await _appCtx.Customers.Where(c => c.Id == customerId).AnyAsync()) return new NotFoundResult();

            DateTime? from = GetDateFromQuery("from"), to = GetDateFromQuery("to");
            if (from == null || to == null) return new BadRequestResult();

            return new OkObjectResult(await _tripsQueryService.GetSummaryFromCustomer(customerId, from, to));
        }
        
        [HttpGet]
        [Route("customer/{customerId:int}/multi-summary")]
        public async Task<ActionResult<MultiTripSummary>> GetMultiSummaryFromCustomer(
            [FromRoute(Name = "customerId")] int customerId,
            [FromQuery(Name = "skip")] int skip = 0,
            [FromQuery(Name = "take")] int take = 15
        )
        {
            if (!await _appCtx.Customers.Where(c => c.Id == customerId).AnyAsync()) return new NotFoundResult();

            DateTime? from = GetDateFromQuery("from"), to = GetDateFromQuery("to");
            if (from == null || to == null) return new BadRequestResult();

            return new OkObjectResult(await _tripsQueryService.GetMultiTripSummaryFromCustomer(customerId, from, to));
        }
        
        [HttpGet]
        [Route("customer/{customerId:int}/pending-summary")]
        public async Task<ActionResult<CustomerPendingSummary>> GetCustomerPendingSummary(
            [FromRoute(Name = "customerId")] int customerId
        )
        {
            if (!await _appCtx.Customers.Where(c => c.Id == customerId).AnyAsync()) return new NotFoundResult();
            
            return new OkObjectResult(await _tripsQueryService.GetCustomerPendingSummary(customerId));
        }


        [HttpPost]
        public async Task<ActionResult<Trip>> Post(CreateTripDto tripData)
        {
            try
            {
                var customer = await _service.CreateTrip(tripData);
                return new OkObjectResult(customer);
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

        [HttpDelete]
        [Route("{tripId:int}")]
        public async Task<ActionResult> Delete([FromRoute(Name = "tripId")] int tripId)
        {
            try
            {
                await _service.DeleteTrip(new DeleteTripDto {TripId = tripId});
                return new OkResult();
            }
            catch (ServiceValidationException ex)
            {
                return new BadRequestObjectResult(ex.result);
            }
            catch (TripNotFoundException)
            {
                return new NotFoundResult();
            }
        }
        
        [HttpPut]
        [Route("{tripId:int}/pay")]
        public async Task<ActionResult<Trip>> Pay([FromRoute(Name = "tripId")] int tripId)
        {
            try
            {
                return new OkObjectResult(await _service.PayTrip(tripId));
            }
            catch (TripNotFoundException)
            {
                return new NotFoundResult();
            }
        }
        
        [HttpPut]
        [Route("{tripId:int}/unpay")]
        public async Task<ActionResult<Trip>> Unpay([FromRoute(Name = "tripId")] int tripId)
        {
            try
            {
                return new OkObjectResult(await _service.UnPayTrip(tripId));
            }
            catch (TripNotFoundException)
            {
                return new NotFoundResult();
            }
        }

        private DateTime? GetDateFromQuery(string key)
        {
            var dateString = HttpContext.Request.Query[key].ToString();
            if (dateString == null) return null;
            return DateTime.TryParse(dateString, out var date) ? date : null;
        }
    }
}