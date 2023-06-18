using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcmeCorp.Core.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ContactInfoId { get; set; }
        public ContactInfo ContactInfo { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
