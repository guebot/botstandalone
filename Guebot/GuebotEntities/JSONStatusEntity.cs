using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuebotEntities
{
    //status: El estado del brazo, puede tomar los siguientes valores: UP_OPEN, UP_CLOSE, DOWN_OPEN, DOWN_CLOSE.
    //code: El código de respuesta, valores válidos: '00' ejecutada exitosamente, '50' problemas de comunicación, '80' petición invalida, '90' petición encolada, '99' error desconocido.
    //message: Mensaje de respuesta, opcional.

    public class JSONStatusEntity
    {
        public consult consult { get; set; }
    }

    public class consult
    {
        public string status { get; set; }
        public response response { get; set; }
    }

    public class response
    {
        public string code { get; set; }
        public string message { get; set; }
    }
}
