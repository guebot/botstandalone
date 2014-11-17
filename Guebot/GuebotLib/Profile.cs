using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuebotLib
{
    public class Profile
    {
        public string Name { get; set; }

        public GuebotComponent Arm { get; set; }

        public GuebotComponent Hand { get; set; }
    }
}
