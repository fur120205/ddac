using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Propmaster.Models
{
    public class CreateListingModel
    {
        public string PropertyId { get; set; }

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
        public string Price { get; set; }

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
        public List<IFormFile> PicUrl { get; set; }

        [Required]
        public string PropertyStatus { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        public List<string> PicUrlList { get; set; }

        public string Urls { get; set; }
    }
}
