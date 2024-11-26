using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace apl_server.Request
{
    public class Message
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public DateTime Date { get; set; } = DateTime.Now;
        
        [Required]
        public string TextMessage { get; set; }
    }
}
