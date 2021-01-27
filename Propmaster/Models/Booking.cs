using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Propmaster.Models
{
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookingId { get; set; }

        [Required]
        [ForeignKey("AspNetUsers")]
        public string ClientId { get; set; }

        [Required]
        public DateTime ReservedDate { get; set; }

        [Required]
        public string PropertyId { get; set; }

        [ForeignKey("AspNetUsers")]
        public string AgentId { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public string Type { get; set; }

        public string Remarks { get; set; }
    }
}
