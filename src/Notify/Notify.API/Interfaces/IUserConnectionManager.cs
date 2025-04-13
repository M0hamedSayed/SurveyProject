namespace Notify.API.Interfaces
{
    public interface IUserConnectionManager
    {
        void AddConnection(string email, string connectionId);
        void RemoveConnection(string connectionId);
        bool TryGetConnectionId(string email, out string connectionId);
        IReadOnlyDictionary<string, string> GetAllConnections();
    }
}
