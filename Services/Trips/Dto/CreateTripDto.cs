using System;

namespace drv_next_api.Services.Trips.Dto
{
    public record CreateTripDto
    {
        public int CustomerId { get; init; }
        public DateTime Date { get; init; }
        public string Origin { get; init; }
        public string Destination { get; init; }
        public Int64 Distance { get; init; }
        public Int64 Fare { get; init; }
        public string Obs { get; init; }
        public DateTime DoneAt { get; init; }
    }
}