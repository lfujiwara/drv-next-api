namespace drv_next_api.Services.Customers.Dto
{
    public record CreateCustomerDto
    {
        public string Name { get; init; }
        public string PhoneNumber { get; init; }
    }
}