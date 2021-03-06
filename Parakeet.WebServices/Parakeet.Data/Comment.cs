//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Parakeet.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Comment
    {
        public Comment()
        {
            this.Tags = new HashSet<Tag>();
            this.MentionedUsers = new HashSet<User>();
            this.MentionedTasks = new HashSet<Task>();
        }
    
        public int Id { get; set; }
        public System.DateTime TimeStamp { get; set; }
        public string Content { get; set; }
        public int UserId { get; set; }
        public Nullable<int> TaskId { get; set; }
    
        public virtual ICollection<Tag> Tags { get; set; }
        public virtual User Author { get; set; }
        public virtual ICollection<User> MentionedUsers { get; set; }
        public virtual Task ParentTask { get; set; }
        public virtual ICollection<Task> MentionedTasks { get; set; }
    }
}
