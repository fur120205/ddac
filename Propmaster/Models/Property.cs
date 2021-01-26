using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Propmaster.Models
{
    public class Property
    {
        public int PropertyId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PropertySize { get; set; }
        public string PropertyLocation { get; set; }
        public string PropertyArea { get; set; }
        public string PropertyType { get; set; }
        public string Price { get; set; }
        public string Furnished { get; set; }
        public string Bedroom { get; set; }
        public string Bathroom { get; set; }
        public string Carpark { get; set; }
        public string CoverUrl { get; set; }
        public string PicUrl { get; set; }
        public string CreatedBy { get; set; }
        public string PropertyStatus { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
