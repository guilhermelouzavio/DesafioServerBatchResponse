using apl_server.Request;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Data.Common;
using System.Text;
using System.Threading.Channels;

namespace apl_server.Client
{
    public abstract class RabbitMqClient
    {

        private TaskCompletionSource<Message> _task;
        private IConnection? _connection;

        public async Task CreateTaskSource()
        {
            _task = new TaskCompletionSource<Message>();
        }
        public Message GetResultTask() => _task.Task.Result;
        private async Task WaitResponseMessage() => await _task.Task;     
        private void SetMessageConfirmed(Message response) => _task.SetResult(response);
        

        public async Task CreateConnection(ConnectionFactory connectionFactory)
        {
            try
            {
                if (_connection is null || _connection.IsOpen == false)
                    _connection = await connectionFactory.CreateConnectionAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<QueueDeclareOk> CreateQueue(string queueName)
        {
            try
            {
                QueueDeclareOk queue;
                var channel = await _connection.CreateChannelAsync();
                queue = await channel.QueueDeclareAsync(queueName, true, false, false);
                await CloseChannel(channel);
                return queue;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task WriteMessageOnQueue(string message, string queueName, string requestID)
        {
            
            var channel = await _connection.CreateChannelAsync();
            try
            {

                byte[] messageBodyBytes = Encoding.UTF8.GetBytes(message);

                var props = new BasicProperties();
                props.CorrelationId = requestID;
                props.ContentType = "aplication/json";
                props.DeliveryMode = DeliveryModes.Persistent;

                await channel.BasicPublishAsync(string.Empty, queueName,false, props, messageBodyBytes);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                await CloseChannel(channel);
            }
        
        }

        public async Task ReadMessageQueue(string queueName, string requestID) 
        {
            var channel = await _connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queueName, true, false, false, null);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, args) =>
            {
                try
                {
                    if (requestID.Equals(args.BasicProperties.CorrelationId))
                    {
                        var response = Encoding.UTF8.GetString(args.Body.ToArray());

                        this.SetMessageConfirmed(JsonConvert.DeserializeObject<Message>(response));

                        await channel.BasicAckAsync(args.DeliveryTag, false);

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

        public async Task CloseConnection() 
        {
            try
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task CloseChannel(IChannel channel)
        {
            try
            {
                await channel.CloseAsync();
                await channel.DisposeAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
