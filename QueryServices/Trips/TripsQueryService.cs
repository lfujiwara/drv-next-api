using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using drv_next_api.Data;
using drv_next_api.Models;
using drv_next_api.QueryServices.Trips.Dto;
using Microsoft.EntityFrameworkCore;

namespace drv_next_api.QueryServices.Trips
{
    public class TripsQueryService
    {
        private readonly ApplicationContext _ctx;

        public TripsQueryService(ApplicationContext ctx)
        {
            _ctx = ctx;
        }

        public IQueryable<Trip> GetTripsWithinDateRange(DateTime? from = null, DateTime? to = null)
        {
            var queryable = _ctx.Trips.AsQueryable();

            if (from != null)
                queryable = queryable.Where(
                    c => c.Date >= from);
            if (to != null)
                queryable = queryable.Where(
                    c => c.Date <= to);

            return queryable;
        }

        public IQueryable<Trip> GetTripsFromCustomer(int customerId, DateTime? from = null, DateTime? to = null)
        {
            var queryable = GetTripsWithinDateRange(from, to).Where(c => c.CustomerId == customerId);
            return queryable.OrderBy(t => t.Date);
        }

        private static async Task<TripSummary> GetSummary(IQueryable<Trip> queryable)
        {
            var count = await queryable.OrderBy(t => t.Date).CountAsync();

            return new TripSummary
            {
                Count = count,
                FareAvg = count == 0 ? 0 : await queryable.AverageAsync(t => t.Fare),
                FareSum = count == 0 ? 0 : await queryable.SumAsync(t => t.Fare),
                DistanceAvg = count == 0 ? 0 : await queryable.AverageAsync(t => t.Distance),
                DistanceSum = count == 0 ? 0 : await queryable.SumAsync(t => t.Distance)
            };
        }

        public Task<TripSummary> GetSummary(DateTime? from = null, DateTime? to = null)
        {
            return GetSummary(GetTripsWithinDateRange(from, to));
        }

        public Task<TripSummary> GetSummaryFromCustomer(int customerId, DateTime? from = null, DateTime? to = null)
        {
            return GetSummary(GetTripsFromCustomer(customerId, from, to));
        }
        
        public Task<MultiTripSummary> GetMultiTripSummaryFromCustomer(int customerId, DateTime? from = null, DateTime? to = null)
        {
            return GetMultiTripSummary(GetTripsFromCustomer(customerId, from, to));
        }
        
        public Task<MultiTripSummary> GetMultiTripSummary(DateTime? from = null, DateTime? to = null)
        {
            return GetMultiTripSummary(GetTripsWithinDateRange(from, to));
        }

        public async Task<MultiTripSummary> GetMultiTripSummary(IQueryable<Trip> queryable)
        {
            var total = queryable;
            var paid = queryable.Where(t => t.Paid != null);
            var unpaid = queryable.Where(t => t.Paid == null);

            return new MultiTripSummary
            {
                Total = await GetSummary(total),
                Paid = await GetSummary(paid),
                Unpaid = await GetSummary(unpaid)
            };
        }

        public async Task<CustomerPendingSummary> GetCustomerPendingSummary(int customerId)
        {
            var trips =  GetTripsFromCustomer(customerId).Where(t => t.Paid == null).OrderBy(t => t.Date);
            var count = await trips.CountAsync();

            var monthsWithPendingTrips = new HashSet<string>();
            foreach (var trip in trips)
            {
                var year = trip.Date.Year.ToString("0000");
                var month = trip.Date.Month.ToString("00");
                monthsWithPendingTrips.Add(year + '-' + month);
            }

            var earliestTrip = await trips.FirstOrDefaultAsync();
            var from = earliestTrip?.Date;
            var total = await trips.SumAsync(t => t.Fare);

            var result = new CustomerPendingSummary
            {
                Count = count,
                From = from,
                Total = total,
                MonthsWithPendingTrips = monthsWithPendingTrips.ToList()
            };

            return result;
        }
    }
}