using RabbitMQ.Client;
using System.Text;
using System.Threading.Channels;

namespace apl_batch_readQueue.Client
{
    public abstract class RabbitMqClient
    {
        private IConnection? _connection;
        public async Task CreateConnection(ConnectionFactory connectionFactory)
        {
            try
            {
                if (_connection is null || _connection.IsOpen == false)
                {
                    _connection = await connectionFactory.CreateConnectionAsync();
                }
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

                await channel.BasicPublishAsync(string.Empty, queueName, false, props, messageBodyBytes);

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

        public async Task<(string message, string correlationID)?> ReadMessageQueue(string queueName)
        {
            (string message, string correlationID)? tuplaReponse = null;

            var channel = await _connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queueName, true, false, false, null);

            var eventMessage = await channel.BasicGetAsync(queueName, false);

            try
            {
                if (eventMessage is not null)
                {
                    var props = eventMessage.BasicProperties;
                    var message = Encoding.UTF8.GetString(eventMessage.Body.ToArray());
                    await channel.BasicAckAsync(eventMessage.DeliveryTag, false);

                    tuplaReponse = (message, props?.CorrelationId);

                    return tuplaReponse;
                }
            }
            catch (Exception)
            {
                await channel.BasicNackAsync(eventMessage.DeliveryTag, false, true);
                throw;
            }
            finally
            {
                await CloseChannel(channel);
            }
            

            return tuplaReponse;

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
