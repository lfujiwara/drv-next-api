namespace drv_next_api.QueryServices.Trips.Dto
{
    public record TripSummary
    {
        public int Count { get; init; }
        public long FareSum { get; init; }
        public double FareAvg { get; init; }
        public long DistanceSum { get; init; }
        public double DistanceAvg { get; init; }
    }
}