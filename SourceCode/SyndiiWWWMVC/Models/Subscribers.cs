using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SyndiiWWWMVC.Models
{
    public class Subscriber
    {
        public int ID { get; set; }

        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "The email format is invalid")]
        [Required(ErrorMessage ="Please enter your email address.")]
        public string Email { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime? DateRemoved { get; set; }

        public Boolean IsActive { get; set; }

        [NotMapped]
        [Compare("Email", ErrorMessage = "The email addresses do not match.")]
        public string ConfirmEmail { get; set; }

        [NotMapped]
        [Required (ErrorMessage = "You must accept the Terms of Use to continue.")]
        [Display(Name = "Terms of Use")]
        public bool TermsAndConditions { get; set; }
    }
}
