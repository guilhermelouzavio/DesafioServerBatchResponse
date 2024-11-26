using apl_server.Request;

namespace apl_server.Service
{
    public interface IMessageService
    {
        Task<object> ProcessaMensagem(Message request);
    }
}
