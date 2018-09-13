using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Wall.Models
{
    public class Message : BaseEntity
    {
        [Key]
        public int message_id {get;set;}
        public string message {get;set;}
        public int user_id {get;set;}
        public User Users {get;set;}
        public List<Comment> Comments {get;set;}
        public Message()
        {
            Comments = new List<Comment>();
            created_at = DateTime.Now;
            updated_at = DateTime.Now;
        }
    }
}