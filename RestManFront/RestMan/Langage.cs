using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestMan
{
    public class Langage
    {
        private static string lang = "Fr";

        public static string Lang
        {
            get { return lang; }
            set { lang = value; }
        }
    }
}
