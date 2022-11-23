using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using Base64 = System.Buffers.Text.Base64;

namespace ApiSIES.Models
{
    public class Alumno
    {
        public int ClaveUnica { get; set; }

        public int Semestre { get; set; }
        public string NombreCompleto { get; set; }
        public string fotografia { get; set; }
        public int IdDES { get; set; }
        public int IdCarrera { get; set; }
        public int Generacion { get; set; }
        public bool Regular { get; set; }
        public bool MateriasIncritas { get; set; }

    }
}
