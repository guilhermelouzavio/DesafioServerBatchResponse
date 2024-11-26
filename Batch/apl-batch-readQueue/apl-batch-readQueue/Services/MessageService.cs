using apl_batch_readQueue.Client;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apl_batch_readQueue.Services
{
    internal class MessageService : RabbitMqClient , IMessageService
    {
        private readonly string SEND_QUEUE = "message_send_queue";
        private readonly string RESPONSE_QUEUE = "message_response_queue";
        public async Task ProcessaMensagem()
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

                Console.WriteLine("Iniciando Leitura de Mensagens na Fila...");

                var tuplaResponse = await base.ReadMessageQueue(SEND_QUEUE);
                Console.WriteLine(tuplaResponse);

                if (tuplaResponse is not null)
                {
                    Console.WriteLine("Escrevendo mensagem na fila de entrega...");
                    await base.WriteMessageOnQueue(tuplaResponse?.message, RESPONSE_QUEUE, tuplaResponse?.correlationID);
                }

                await base.CloseConnection();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
