using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Propmaster.Models
{
    public class Filter
    {
        public string MinPrice { get; set; }
        public string MaxPrice { get; set; }
        public string PropertyLocation { get; set; }
        public string PropertyType { get; set; }
        public int Bedroom { get; set; }
        public List<Property> PropertyList{ get; set; }
    }
}
