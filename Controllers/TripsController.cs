using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using drv_next_api.Controllers.Models;
using drv_next_api.Data;
using drv_next_api.Models;
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
        private readonly ILogger<TripsController> _logger;
        private readonly TripsService _service;
        private readonly ApplicationContext _appCtx;

        public TripsController(ILogger<TripsController> logger, ApplicationContext appCtx, TripsService service)
        {
            _logger = logger;
            _appCtx = appCtx;
            _service = service;
        }

        [HttpGet]
        public ActionResult<PagedResult<Trip>> Get([FromQuery(Name = "skip")] int skip = 0, [FromQuery(Name = "take")] int take = 15)
        {
            return new OkObjectResult(new PagedResult<Trip>(take, skip, _appCtx.Trips.OrderBy(k => k.CustomerId).Skip(skip).Take(take).ToList()));
        }

        [HttpGet]
        [Route("customer/{customerId}")]
        public async Task<ActionResult<PagedResult<Trip>>> GetFromCustomer(
            [FromRoute(Name = "customerId")] int customerId,
            [FromQuery(Name = "skip")] int skip = 0,
            [FromQuery(Name = "take")] int take = 15,
            [FromQuery(Name = "fromMonth")] string fromMonth = "",
            [FromQuery(Name = "toMonth")] string toMonth = ""
        )
        {
            int startYear = 0, startMonth = 0, endYear = 0, endMonth = 0;
            bool hasStartYear = false, hasStartMonth = false, hasEndYear = false, hasEndMonth = false;

            var start = fromMonth.Split('/');
            if (start.Length >= 2)
            {
                hasStartYear = int.TryParse(start[0], out startYear);
                hasStartMonth = int.TryParse(start[1], out startMonth);
            }
            var end = toMonth.Split('/');
            if (end.Length >= 2)
            {

                hasEndYear = int.TryParse(end[0], out endYear);
                hasEndMonth = int.TryParse(end[1], out endMonth);
            }

            var now = DateTime.UtcNow;

            startYear = hasStartYear ? startYear : now.Year;
            startMonth = hasStartMonth ? startMonth : now.Month;
            endYear = hasEndYear ? endYear : now.Year;
            endMonth = hasEndMonth ? endMonth : now.Month;

            var startPeriod = new DateTime(startYear, startMonth, 1);
            var endPeriod = new DateTime(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth));

            var trips = await _appCtx.Trips.Where(t => t.CustomerId == customerId && startPeriod <= t.Date && t.Date <= endPeriod).OrderByDescending(t => t.Date).Skip(skip).Take(take).ToListAsync();
            return new OkObjectResult(new PagedResult<Trip>(take, skip, trips));
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
        [Route("{tripId}")]
        public async Task<ActionResult> Delete([FromRoute(Name = "tripId")] int tripId)
        {
            try
            {
                await _service.DeleteTrip(new DeleteTripDto { TripId = tripId });
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
    }
}