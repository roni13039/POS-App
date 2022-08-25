using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace POSApplication.Models
{
    [Table("UserImage")]
    public class UserImage
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Secu_User")]
        public int Secu_UserId { get; set; }
        public virtual Secu_User Secu_User { get; set; }

        public DateTime ImageDate { get; set; }
        public string ImageURL { get; set; }

    }
}