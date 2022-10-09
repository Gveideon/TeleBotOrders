using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeleBotOrders
{
    internal class Menu
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<Dish> Dishes { get; set; }  
    }
}
