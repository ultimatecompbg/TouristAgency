using System.ComponentModel.DataAnnotations;

namespace TouristAgency.Models
{
    public class RegisterAgencyViewModel
    {

        [Required]
        [Display(Name = "Телефонен номер")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Адрес")]
        public string Address { get; set; }

        [Required]
        [Display(Name = "Описание на агенцията")]
        public string Description { get; set; }
    }
}
