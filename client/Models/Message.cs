using System;

namespace client.Models
{
    public class Message
    {
        public string Sender { get; set;} 

        public string MessageContent { get; set; }

        public DateTime Timestamp { get; set;}  

        public Message(string sender, string messageContent, DateTime timestamp)
        {
            this.Sender = sender;
            this.MessageContent = messageContent;
            this.Timestamp = timestamp;
        }

    }
}