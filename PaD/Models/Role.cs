using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;

namespace PaD.Models
{
    public enum Role
    {
        [Description("Administrator")]
        Admin,
        [Description("Project Owner")]
        ProjectOwner,
        [Description("User")]
        User
    }
}