using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parakeet.DataModel;
using System.IO;

namespace Parakeet.Sessions
{
    // TODO: Move this to the domain services. 
    public class MessageRepository
    {
        #region Constants
        private const string tempFile = "CurrentThoughts.txt";
        private const string logFile = "ThoughtStream.txt";
        #endregion

        internal void WriteStaleMessage(Message message)
        {
            if (!File.Exists(logFile))
            {
                using (StreamWriter writer = File.CreateText(logFile))
                {
                    writer.WriteLine(DateTime.Now.ToString() + ":");
                    writer.WriteLine(message.Caption);
                    writer.WriteLine();
                }
            }
            else
            {
                using (StreamWriter writer = File.AppendText(logFile))
                {
                    writer.WriteLine(DateTime.Now.ToString() + ":");
                    writer.WriteLine(message.Caption);
                    writer.WriteLine();
                }
            }
        }

        internal void SaveThoughts(IEnumerable<Message> messages)
        {
            using (StreamWriter writer = File.CreateText(tempFile))
            {
                foreach (var message in messages)
                {
                    writer.WriteLine(DateTime.Now.ToString() + ":");
                    writer.WriteLine(message.Caption);
                    writer.WriteLine();
                }
            }
        }

        internal string[] LoadThoughts()
        {
            string[] lines = new string[0];
            if (File.Exists(tempFile))
            {
                using (StreamReader reader = File.OpenText(tempFile))
                {
                    lines = reader.ReadToEnd().Split('\n');
                }
            }
            return lines;
        }
    }
}
