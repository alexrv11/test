using System;

namespace BGBA.Services.N.Autenticacion.SCS
{
    internal class SemillaAutenticacion
    {
        public String Key { get; set; }
        public String Id { get; set; }

        public SemillaAutenticacion(String key, String id)
        {
            Key = key;
            Id = id;
        }

        public SemillaAutenticacion() { }
    }
}
