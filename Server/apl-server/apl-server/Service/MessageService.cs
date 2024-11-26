
using apl_server.Client;
using System.Text.Json;
using RabbitMQ.Client;
using System.Diagnostics;
using apl_server.Request;
using Newtonsoft.Json;

namespace apl_server.Service
{
    public class MessageService : RabbitMqClient, IMessageService
    {

        private readonly string SEND_QUEUE = "message_send_queue";
        private readonly string RESPONSE_QUEUE = "message_response_queue";

        public async Task<object> ProcessaMensagem(Message request)
        {
            try
            {
                var connectionFactory = new ConnectionFactory()
                {
                    HostName = "localhost",
                    UserName = "guest",
                    VirtualHost = "/"

                };

                await base.CreateConnection(connectionFactory);
                await base.CreateQueue(SEND_QUEUE);
                await base.CreateQueue(RESPONSE_QUEUE);

                await base.WriteMessageOnQueue(JsonConvert.SerializeObject(request), SEND_QUEUE, request.ID.ToString());

                await base.CreateTaskSource();

                var clock = new Stopwatch();
                clock.Start();

                await base.ReadMessageQueue(RESPONSE_QUEUE, request.ID.ToString());

                clock.Stop();
                var timeWaiting = clock.Elapsed;
                var message = base.GetResultTask();

                return new { message, timeWaiting };
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
