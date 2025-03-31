
using Microsoft.EntityFrameworkCore.Storage;

namespace Survey.Domain.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        //repositories
        IUserRepository UserRepository { get; }
        IRefreshTokenRepository RefreshTokenRepository { get; }
        ISurveyRepository SurveyRepository { get; }
        ISurveyTypeRepository SurveyTypeRepository { get; }
        public IQuestionSurveyRepository QuestionSurveyRepository { get; }
        public IChoiceRepository ChoiceRepository { get; }
        public IEvaluateChoiceRepository EvaluateChoiceRepository { get; }
        public ISurveyResponseRepository ResponseRepository { get; }
        IDbContextTransaction BeginTransaction();
        void Commit();
        void RollBack();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitAsync();
        Task RollBackAsync();
        // save changes
        Task Complete();
    }
}
