using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfinityBooksFunctionApp.Models
{
    class ProductImage
    {
        [Key]
        public int productImageId { get; set; }
        public int productId { get; set; }
        public string blobimagePath { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public DateTime? deletedAt { get; set; }
    }
}
