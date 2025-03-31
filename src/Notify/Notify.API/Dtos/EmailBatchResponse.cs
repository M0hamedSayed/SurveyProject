namespace Notify.API.Dtos
{
    public record EmailBatchResponse
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public bool Succeeded { get; set; }
        public DataResult? Data { get; set; }
    }
    public record DataResult
    {
        public List<string?>? List { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public bool hasNextPage { get; set; }
    }
}
