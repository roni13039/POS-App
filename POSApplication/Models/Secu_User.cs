using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace POSApplication.Models
{
    public class Secu_User
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
        public string Salt { get; set; }

        [Display(Name = "Full Name")]
        public string UserFullName { get; set; }

        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }
        
        [Display(Name = "Is Admin")]
        public bool IsAdmin { get; set; }

        //[Required]
        [Display(Name = "User Status")]
        public string UserStatus { get; set; }

        [Display(Name = "Last Login")]
        public DateTime? LastLoginDate { get; set; }
        public int? InvalidAttempt { get; set; }

        [Display(Name = "Role Name")]
        [ForeignKey("Secu_Role")]
        public int? RoleId { get; set; }
        public virtual Secu_Role Secu_Role { get; set; }
        //public bool? IsSuperAdmin { get; set; }
        

    }
}