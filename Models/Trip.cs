using System;

namespace drv_next_api.Models
{
    public class Trip
    {
        public int Id { get; private set; }
        public int CustomerId { get; private set; }
        public Customer Customer { get; private set; }
        public DateTime Date { get; private set; }
        public string Origin { get; private set; }
        public string Destination { get; private set; }
        public Int64 Distance { get; private set; }
        public Int64 Fare { get; private set; }
        public string Obs { get; private set; }
    }
}