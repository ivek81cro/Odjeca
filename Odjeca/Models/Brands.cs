using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace Odjeca.Models
{
    public class Brands
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
        
        public byte[] Image { get; set; }
    }
}
