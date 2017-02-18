using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parakeet.DataModel;
using System.IO;

namespace Parakeet.Sessions
{
    public class MessageReviewSession : SessionBase
    {
        private readonly Action<Message> messageReviewed;
        private readonly MessageRepository messageRepository;

        public MessageReviewSession(Message message, Action<Message> messageReviewed)
        {
            this.messageReviewed = messageReviewed;
            Message = message;
            messageRepository = new MessageRepository();
        }

        public Message Message { get; private set; }

        public void RepostCurrent()
        {
            var current = Message;
            var @new = (Message)current.Clone();
            @new.CreatedAt = DateTime.Now;
            SaveMessage(Message);
            messageReviewed(@new);

        }
        public void DismissCurrent()
        {
            SaveMessage(Message);
            messageReviewed(Message);
        }

        private void SaveMessage(Message message)
        {
            message.Status = Status.Stale;
            messageRepository.WriteStaleMessage(message);
        }
    }
}
