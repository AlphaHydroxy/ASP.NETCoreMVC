using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet.ViewModels
{
    public class ContactViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(4906, MinimumLength = 5)]
        public string Message { get; set; }
        //the purpose of this class is to be able to communicate with the contact page and get the data back as a series of fields
        //by using a class we are using something called model binding
        //this accepts the data from the form directly inside a method of the controller
    }
}
