using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Odjeca.Models
{
    public class StoreItem
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        [Display(Name = "Brand")]
        public string Manufacturer { get; set; }

        public string Image { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [Display(Name = "Sub Category")]
        public int SubCategoryId { get; set; }

        [ForeignKey("SubCategoryId")]
        public virtual SubCategory SubCategory { get; set; }

        [Display(Name = "Price $")]
        [Range(0.01, int.MaxValue, ErrorMessage = "Price should be greater than 0.01$")]
        public double Price { get; set; }

        [Display(Name = "Arrival Date")]
        public DateTime ArrivalDate { get; set; }
    }
}
