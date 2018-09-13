using System;
using System.ComponentModel.DataAnnotations;

namespace Wall.Models
{
    public class Comment : BaseEntity
    {
        [Key]
        public int comment_id {get;set;}
        public int user_id {get;set;}
        public User Users {get;set;}
        public int message_id {get;set;}
        public Message Messages {get;set;}
        public string comment {get;set;}
        public Comment()
        {
            created_at = DateTime.Now;
            updated_at = DateTime.Now;
        }
    }
}