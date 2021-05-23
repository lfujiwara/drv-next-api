using System;

namespace drv_next_api.Services.Trips.Dto
{
    public record DeleteTripDto
    {
        public int TripId { get; init; }
    }
}