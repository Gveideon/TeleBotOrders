using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeleBotOrders
{
    internal class Order
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public Cafe Cafe { get; set; }
        public Menu Menu { get; set; }
        public  List<User> Users { get; set; }
        public List<Dish> Dishes { get; set; }

    }
}
