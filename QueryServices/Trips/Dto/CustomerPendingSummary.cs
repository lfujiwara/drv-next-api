using System;
using System.Collections.Generic;

namespace drv_next_api.QueryServices.Trips.Dto
{
    public record CustomerPendingSummary
    {
        public int Count { get; init; }
        public long Total { get; init; }
        public DateTime? From { get; init; }
        public IEnumerable<string> MonthsWithPendingTrips { get; init; }
    }
}