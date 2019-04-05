using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestMan.Objects
{
    public class AuthenticationSave
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Libelle { get; set; }
        public DateTime Date { get; set; }
    }
}
