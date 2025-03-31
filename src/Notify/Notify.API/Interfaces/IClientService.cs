using Notify.API.Dtos;

namespace Notify.API.Interfaces
{
    public interface IClientService
    {
        public Task<EmailBatchResponse?> GetUsersBatchAsync(Guid surveyId, int pageNumber, int pageSize, bool allusers);
    }
}
