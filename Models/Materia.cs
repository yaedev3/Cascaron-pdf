using Newtonsoft.Json;
using System;

namespace ApiSIES.Models
{
    public class Materia
    {
        public int IdDetalleMateria { get; set; }

        public int IdTipoDetalleGrupo { get; set; }

        public int Creditos { get; set; }

        public string Nombre { get; set; }
        public int IdCiclo { get; set; }
        public string NombreCiclo { get; set; }
        public string NombrePeriodo { get; set; }
        public string IdTipoExamen { get; set; }
        public string Calificacion { get; set; }
        public string TipoDetalleGrupo { get; set; }
        public DateTime Fecha { get; set; }
        public string Acta { get; set; }

        public string EO1 { get; set; }

        public string EE1 { get; set; }

        public string ET1 { get; set; }

        public string EO2 { get; set; }

        public string EE2 { get; set; }

        public string ET2 { get; set; }

        public string EO3 { get; set; }

        public string EE3 { get; set; }

        public string ET3 { get; set; }

        public string ER1 { get; set; }

        public string ER2 { get; set; }

        public string ER3 { get; set; }

    }
}
