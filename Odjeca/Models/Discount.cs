using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Odjeca.Models
{
    public class Discount
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string DiscountType { get; set; }

        public enum EDiscountType { Percent=0, Dollar=1 }

        [Required]
        public double DiscountAmmount { get; set; }

        [Required]
        public double MinimumAmmount { get; set; }
        
        //saving picture to database as byte of streams
        public byte[] Picture { get; set; }

        public bool IsActive { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
    }
}
