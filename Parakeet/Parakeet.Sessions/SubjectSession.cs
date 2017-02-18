using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parakeet.DataModel;
using System.IO;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Timers;

namespace Parakeet.Sessions
{
    public class SubjectSession : SessionBase
    {
        #region Fields
        private readonly MessageRepository messageRepository;
        #endregion

        #region Constructor
        public SubjectSession(Subject subject)
        {
            messageRepository = new MessageRepository();
            this.Subject = subject;
            ActiveThoughts = new ObservableCollection<Message>();
            StaleThoughts = new ObservableCollection<Message>();

            Timer timer = new Timer(TimeSpan.FromMinutes(5).TotalMilliseconds);
            timer.Elapsed += timer_Elapsed;
            timer.Start();

            string[] lines = messageRepository.LoadThoughts();
            for (int index = 1; index < lines.Length; index += 3)
                PostThought(lines[index].TrimEnd(Environment.NewLine.ToCharArray()), false);
        }
        #endregion

        #region Timer Action
        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ApplicationSession.Invoker.BeginInvoke(new Action(() =>
            {
                if (ActiveThoughts.Count == 0)
                    return;

                if (ReviewSession != null)
                    ReviewSession.RepostCurrent();

                Message message = ActiveThoughts[ActiveThoughts.Count - 1];
                ActiveThoughts.Remove(message);
                RaiseUpdate("ActiveThoughts");

                StaleThoughts.Add(message);
                RaiseUpdate("StaleThoughts");

                ReviewSession = new MessageReviewSession(message, result =>
                {
                    if (result.Status == Status.Active)
                    {
                        ActiveThoughts.Insert(0, result);
                        RaiseUpdate("ActiveThoughts");
                    }
                    ReviewSession = null;
                    RaiseUpdate("ReviewSession");
                });
                RaiseUpdate("ReviewSession");
            }), null);
        }
        #endregion

        #region Properties
        public Subject Subject { get; private set; }
        public MessageReviewSession ReviewSession { get; private set; }
        #endregion

        #region Methods
        public string PendingThought { get; set; }
        public void PostThought()
        {
            PostThought(PendingThought, true);

        }

        private void PostThought(string caption, bool saveToFile)
        {
            if (string.IsNullOrEmpty(caption))
                return;

            Message message = new Message
                {
                    ID = Guid.NewGuid(),
                    Caption = caption,
                    CreatedAt = DateTime.Now,
                    ReviewedAt = DateTime.Now,
                    Status = Status.Active,
                    Subject = this.Subject
                };


            ActiveThoughts.Add(message);
            if (saveToFile)
            {
                ActiveThoughts.Insert(0, message);
                messageRepository.SaveThoughts(ActiveThoughts);
            }

            PendingThought = string.Empty;
            RaiseUpdate("PendingThought");
            RaiseUpdate("ActiveThoughts");
        }
        #endregion

        #region Stale Thoughts
        public ObservableCollection<Message> StaleThoughts { get; private set; }
        public Message FocusedStaleThought { get; set; }
        public void RechirpFocusedStaleThought()
        {
            Message message = FocusedStaleThought;

            // TODO: Add availabilities
            if (message == null)
                return;

            throw new NotImplementedException("RechirpFocusedStaleThought");

            //message.Status = Status.Stale;
            //messageRepository.SaveStaleMessage(message);
            //ActiveThoughts.Remove(message);
            //StaleThoughts.Add(message);

            RaiseUpdate("ActiveThoughts");
        }
        #endregion

        #region Active Thoughts
        public ObservableCollection<Message> ActiveThoughts { get; private set; }
        public Message FocusedActiveThought { get; set; }
        public void DismissFocusedActiveThought()
        {
            Message message = FocusedActiveThought;

            // TODO: Add availabilities
            if (message == null)
                return;

            message.Status = Status.Stale;
            messageRepository.WriteStaleMessage(message);
            ActiveThoughts.Remove(message);
            StaleThoughts.Add(message);

            messageRepository.SaveThoughts(ActiveThoughts);
            RaiseUpdate("ActiveThoughts");
        }
        #endregion
    }
}
