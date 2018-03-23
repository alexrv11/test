﻿using System;

namespace Models.N.Consulta.Padron
{
    public class DatosPadron
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Sexo { get; set; }
        public DateTime FechaDeNacimiento { get; set; }
        public string Cuix { get; set; }
        public string TipoCuix { get; set; }
    }
}
