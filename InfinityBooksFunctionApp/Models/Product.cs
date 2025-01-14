﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfinityBooksFunctionApp.Models
{
    class Product
    {
        [Key]
        public int productId { get; set; }
        public string productCode { get; set; }
        public int productTypeId { get; set; }
        public int productCategoryId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string author { get; set; }
        public string publisher { get; set; }
        public DateTime? publishDate { get; set; }
        public string quantity { get; set; }
        public float price { get; set; }
        public float saleprice { get; set; }
        public float avgRating { get; set; }
        public int? status { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public DateTime? deletedAt { get; set; }

        [NotMapped]
        public List<ProductImage> productImages { get; set; }
        [NotMapped]
        public string productCategoryName { get; set; }
        [NotMapped]
        public string productTypeName { get; set; }
    }
}
