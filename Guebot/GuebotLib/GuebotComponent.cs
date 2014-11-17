using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuebotLib
{
    public class GuebotComponent
    {
        public int Id { get; set; }

        public int MinValue { get; set; }

        public int MaxValue { get; set; }

        public int ActualValue { get; set; }

        public int StepMovement { get; set; }
    }
}
