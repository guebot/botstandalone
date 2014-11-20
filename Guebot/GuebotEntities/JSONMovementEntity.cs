using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuebotEntities
{
//    token: Token generado por google.
//    userId: El id de usuario.
//    instruction: La instrucción a enviar al brazo robótico. Valores válidos: UP, DOWN, CLOSE, OPEN.

    public class JSONMovementEntity
    {
        public move move { get; set; }
    }

    public class move
    {
        public string token { get; set; }
        public string userId { get; set; }
        public data data { get; set; }
    }

    public class data
    {
        public string instruction { get; set; }
        public string value { get; set; }
    }
}
