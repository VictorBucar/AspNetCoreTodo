using AspNetCoreTodo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreTodo.ViewModels.ManageUsersViewModel
{
    public class ManageUsersViewModel
    {
        public ApplicationUser[] Administrators { get; set; }
        public ApplicationUser[] Everyone { get; set; }
    }
}
