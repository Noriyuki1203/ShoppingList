using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingList.ViewModels
{
    public class Item
    {
        public Item() { 
            Id = string.Empty;
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            PublishedAt = DateTime.Now;
            RevisedAt = DateTime.Now;
            Name = string.Empty;
            IsUrgen = false;
        }

        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime PublishedAt { get; set; }
        public DateTime RevisedAt { get; set; }
        public string Name { get; set; }
        public bool IsUrgen { get; set; }
    }
}
