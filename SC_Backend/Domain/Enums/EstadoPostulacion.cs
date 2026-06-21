using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum EstadoPostulacion
    {
        Inscrito = 18,
        AptoEvaluacionCurricular = 19,
        NoApto = 20,          // Ajusta el ID según tu tabla física
        AptoEntrevista = 21,  // Ajusta el ID según tu tabla física
        Ganador = 22          // Ajusta el ID según tu tabla física
    }
}
