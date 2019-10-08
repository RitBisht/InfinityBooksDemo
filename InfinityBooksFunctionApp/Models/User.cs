using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfinityBooksFunctionApp.Models
{
    class User
    {
        [Key]
        public string id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string emailId { get; set; }
        public string status { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
        public string deletedAt { get; set; }
    }
}
