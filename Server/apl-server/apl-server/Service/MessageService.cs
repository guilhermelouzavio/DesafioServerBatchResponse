
using apl_server.Client;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace apl_server.Service
{
    public class MessageService : RabbitMqClient, IMessageService
    {

        private readonly string SEND_QUEUE = "message_send_queue";
        private readonly string RESPONSE_QUEUE = "message_response_queue";

        public async Task<object> ProcessaMensagem(object request)
        {

            var connectionFactory = new ConnectionFactory()
            { 
                HostName = "localhost",
                UserName = "guest",
                VirtualHost = "/"
            
            };

            var connection = await base.CreateConnection(connectionFactory);
            var retorno = await base.CreateQueue(SEND_QUEUE, connection);
          
            await base.CreateQueue(RESPONSE_QUEUE, connection);

            var codigoMensagem = Guid.NewGuid().ToString(); 
            
            var objectRequest = new { 
                ID = codigoMensagem,
                Message = request
            };

            await base.WriteMessageOnQueue(JsonConvert.SerializeObject(objectRequest), SEND_QUEUE, connection, codigoMensagem);

            var confirmationTask = base.CreateTaskSource();
           
            await base.ReadMessageQueue(SEND_QUEUE, connection, codigoMensagem);

            return confirmationTask.Result;


        }
    }
}
