using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleAuthWithPostGres.Models
{

    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Display(Name = "Active User")]
        public bool isActive { get; set; }
        [Display(Name = "User Type")]
        public UserType UserTypeInt { get; set; }
        public DateTime LoggedInDateTime { get; set; }
    }

    public enum UserType
    {
        ReadOnly = 1,
        Admin = 2,
        SuperAdmin = 3
    }
}
