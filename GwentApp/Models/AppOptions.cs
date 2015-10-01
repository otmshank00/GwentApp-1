using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GwentApp.Models
{
    /// <summary>
    /// This is the class to store the admin page options. Right now it's a simple dictionary
    /// </summary>
    public class AppOptions
    {
        //Design format to store (Dictionary?)
        public Dictionary<string, string> adminOptions
        {
            get; set;
        }
    }
}
