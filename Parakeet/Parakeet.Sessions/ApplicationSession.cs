using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parakeet.DataModel;
using System.ComponentModel;

namespace Parakeet.Sessions
{
    public class ApplicationSession : SessionBase
    {
        #region Global
        public static ApplicationSession instance;
        internal static ISynchronizeInvoke Invoker;
        public static ApplicationSession Start(ISynchronizeInvoke invoker)
        {
            if (instance != null)
                throw new InvalidOperationException("Start");
            ApplicationSession.Invoker = invoker;

            instance = new ApplicationSession();
            return instance;
        }
        #endregion

        private ApplicationSession()
        {
            Subjects = new List<Subject>();
            Subject subject;
            Subjects.Add(subject = new Subject
            {
                Title = "Sample Task"
            });

            MessageManager = new SubjectSession(subject);

         
        }

        public SubjectSession MessageManager { get; private set; }

        public IList<Subject> Subjects { get; private set; }
        public void CreateSubject(string title)
        {
            var subject = new Subject
            {
                ID = Guid.NewGuid(),
                Title = title
            };
            Subjects.Add(subject);
        }
    }
}
