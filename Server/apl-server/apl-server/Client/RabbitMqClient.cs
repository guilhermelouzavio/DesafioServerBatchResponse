using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace apl_server.Client
{
    public abstract class RabbitMqClient
    {

        private TaskCompletionSource<string> _task;

        public Task<string> CreateTaskSource()
        {
            _task = new TaskCompletionSource<string>();
            return _task.Task;
        }
        public async Task WaitResponseMessage()
        {
            await _task.Task;
        }

        public void SetMessageConfirmed(string response)
        {
           _task.SetResult(response);
        }

        public async Task<IConnection> CreateConnection(ConnectionFactory connectionFactory)
        {
            return await connectionFactory.CreateConnectionAsync();          

        }

        public async Task<QueueDeclareOk> CreateQueue(string queueName, IConnection connection)
        {
            QueueDeclareOk queue;
            var channel = await connection.CreateChannelAsync();
            queue = await channel.QueueDeclareAsync(queueName,true,false,false);
            await CloseChannel(channel);

            return queue;
            
        }

        public async Task WriteMessageOnQueue(string message, string queueName, IConnection connection, string requestID)
        {
            try
            {
                var token = CancellationToken.None;
                var channel = await connection.CreateChannelAsync();
                byte[] messageBodyBytes = Encoding.UTF8.GetBytes(message);
                var props = new BasicProperties();
                props.CorrelationId = requestID;
                props.ContentType = "aplication/json";
                props.DeliveryMode = DeliveryModes.Persistent;

                await channel.BasicPublishAsync(string.Empty, queueName,false, props, messageBodyBytes);

                var response = await channel.QueueDeclarePassiveAsync(queueName);

                await CloseChannel(channel);
            }
            catch (Exception)
            {

                throw;
            }
        
        }

        public async Task ReadMessageQueue(string queueName, IConnection connection, string requestID) 
        {
            var response = String.Empty;
            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queueName, true, false, false, null);

            var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.ReceivedAsync += async (model, args) =>
                {
                    try
                    {
                        if (requestID.Equals(args.BasicProperties.CorrelationId))
                        {
                            response = Encoding.UTF8.GetString(args.Body.ToArray());
                            
                            await channel.BasicAckAsync(args.DeliveryTag, false);
                            
                            this.SetMessageConfirmed(response);
                        }
                    }
                    catch (Exception)
                    {
                        await channel.BasicNackAsync(args.DeliveryTag, false, true);
                        throw;
                    }
                };

                await channel.BasicConsumeAsync(queueName, false, consumer);

            await this.WaitResponseMessage();

            await CloseChannel(channel);

        }

        public async Task CloseConnection(IConnection connection) 
        {
            await connection.CloseAsync();
            await connection.DisposeAsync();
        }

        public async Task CloseChannel(IChannel channel)
        {
            await channel.CloseAsync();
            await channel.DisposeAsync();
        }
    }
}
