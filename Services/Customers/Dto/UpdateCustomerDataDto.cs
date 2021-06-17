namespace drv_next_api.Services.Customers.Dto
{
    public record UpdateCustomerDataDto
    {
        public int CustomerId { get; init; }
        public string Name { get; init; }
        public string PhoneNumber { get; init; }
    }
}