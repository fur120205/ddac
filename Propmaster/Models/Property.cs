using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Propmaster.Models
{
    public class Property
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PropertyId { get; set; }

        [Required]
        [ForeignKey("AspNetUsers")]
        public string CreatedBy { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int PropertySize { get; set; }

        [Required]
        public string PropertyLocation { get; set; }

        [Required]
        public string PropertyType { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Required]
        public string Furnished { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int Bedroom { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int Bathroom { get; set; }

        [Column(TypeName = "int")]
        public int Carpark { get; set; }

        [Required]
        public string PicUrl { get; set; }

        [Required]
        public string PropertyStatus { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }
    }
}
