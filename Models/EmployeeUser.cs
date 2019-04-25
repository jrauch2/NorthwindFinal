using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Northwind.Models
{
    //Modify this to add columns to AspNetUsers
    //Run migration afterwards!
    public class EmployeeUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
