using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace POSApplication.Models
{
    public class Secu_Role
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Role Name")]
        public string Name { get; set; }
        public int OpBy { get; set; }
        public DateTime OpOn { get; set; }
    }
}