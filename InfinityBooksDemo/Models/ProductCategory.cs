using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfinityBooksDemo.Models
{
    public class ProductCategory
    {
        [Key]
        public int productCategoriesId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int parentCategory { get; set; }
        public int status { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public DateTime? deletedAt { get; set; }
    }
}
