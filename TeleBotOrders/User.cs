using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeleBotOrders
{
    internal class User
    {
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsInit { get; set; } = false;
        public long? OrderId { get; set; }
        public Order Order { get; set; }
    }
}
