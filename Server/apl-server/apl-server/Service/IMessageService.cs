namespace apl_server.Service
{
    public interface IMessageService
    {
        Task<object> ProcessaMensagem(object request);
    }
}
