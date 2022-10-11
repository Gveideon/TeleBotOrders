using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeleBotOrders
{
    internal class Dish
    {
        [Key]
        public long Id { get; set; } 
        public string Name { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }
        public string PathImage { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public long? MenuId { get; set; }
        public Menu Menu { get; set; }
        public long? OrderId { get; set; }
        public Order Order { get; set; }
    }
}
