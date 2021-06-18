namespace drv_next_api.QueryServices.Trips.Dto
{
    public class MultiTripSummary
    {
        public TripSummary Total { get; set; }
        public TripSummary Paid { get; set; }
        public TripSummary Unpaid { get; set; }
    }
}