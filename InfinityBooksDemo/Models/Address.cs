using System;
using System.ComponentModel.DataAnnotations;

namespace InfinityBooksDemo.Models
{
    public class Address
    {
        [Key]
        public int addressesId { get; set; }
        public int userId { get; set; }
        public int addressTypeId { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string pincode { get; set; }
        public int status { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public DateTime? deletedAt { get; set; }
    }
}
