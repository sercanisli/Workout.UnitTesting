namespace Users.Api.DataTransferObjects
{
    public sealed record UserDtoForInsertion
    {
        public Guid Id { get; init; }
        public string? FullName { get; init; }
    }
}
