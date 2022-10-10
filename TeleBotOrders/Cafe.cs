using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeleBotOrders
{
    internal class Cafe
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public Menu Menu { get; set; }
        public long? OrderId { get; set; }
        public Order Order { get; set; }
    }
}
