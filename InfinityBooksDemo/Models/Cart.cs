using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfinityBooksDemo.Models
{
    public class Cart
    {
        [Key]
        public int id { get; set; }
        public int productId { get; set; }
        public int userId { get; set; }
        public int statusTypeId { get; set; }
        public string description { get; set; }
        public int status { get; set; }
        public int quantity { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public DateTime? deletedAt { get; set; }
        [NotMapped]
        public Product productdetail { get; set; }
    }
}
