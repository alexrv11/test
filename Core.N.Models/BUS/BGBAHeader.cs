using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Core.N.Models.BUS
{
    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("svcutil", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://ws.bancogalicia.com.ar/webservices/globales/bgbaheader/2_0_0")]
    public partial class BGBAHeader
    {

        private Identificadores identificadoresField;

        private ModuloAplicativo moduloAplicativoField;

        private equipo equipoField;

        private Origen origenField;

        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        [JsonProperty("_01:Identificadores")]
        public Identificadores Identificadores
        {
            get
            {
                return this.identificadoresField;
            }
            set
            {
                this.identificadoresField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        [JsonProperty("_01:ModuloAplicativo")]
        public ModuloAplicativo ModuloAplicativo
        {
            get
            {
                return this.moduloAplicativoField;
            }
            set
            {
                this.moduloAplicativoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        [JsonProperty("_01:Equipo", NullValueHandling = NullValueHandling.Ignore)]
        public equipo Equipo
        {
            get
            {
                return this.equipoField;
            }
            set
            {
                this.equipoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
        [JsonProperty("_01:Origen")]
        public Origen Origen
        {
            get
            {
                return this.origenField;
            }
            set
            {
                this.origenField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("svcutil", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://ws.bancogalicia.com.ar/webservices/globales/bgbaheader/2_0_0")]
    public partial class Identificadores
    {

        private idMensaje idMensajeField;

        private idMensaje idMensajeAnteriorField;

        private idMensaje idOperacionField;

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        [JsonProperty("_01:IdMensaje")]
        public idMensaje IdMensaje
        {
            get
            {
                return this.idMensajeField;
            }
            set
            {
                this.idMensajeField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        [JsonProperty("_01:IdMensajeAnterior")]
        public idMensaje IdMensajeAnterior
        {
            get
            {
                return this.idMensajeAnteriorField;
            }
            set
            {
                this.idMensajeAnteriorField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        [JsonProperty("_01:IdOperacion")]
        public idMensaje IdOperacion
        {
            get
            {
                return this.idOperacionField;
            }
            set
            {
                this.idOperacionField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("svcutil", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ws.bancogalicia.com.ar/webservices/globales/bgbaheader/2_0_0")]
    public partial class idMensaje
    {

        private string idEsquemaField;

        private string valueField;

        public idMensaje()
        {
            this.idEsquemaField = "UUI";
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute("UUI")]
        [JsonProperty("@idEsquema")]
        public string idEsquema
        {
            get
            {
                return this.idEsquemaField;
            }
            set
            {
                this.idEsquemaField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlTextAttribute()]
        [JsonProperty("$")]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("svcutil", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ws.bancogalicia.com.ar/webservices/globales/bgbatiposbase/3_0_0")]
    public partial class codigo
    {

        private string idListaField;

        private string organizacionIdListaField;

        private string valueField;

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "token")]
        public string idLista
        {
            get
            {
                return this.idListaField;
            }
            set
            {
                this.idListaField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "token")]
        public string organizacionIdLista
        {
            get
            {
                return this.organizacionIdListaField;
            }
            set
            {
                this.organizacionIdListaField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlTextAttribute(DataType = "token")]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("svcutil", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ws.bancogalicia.com.ar/webservices/globales/bgbatiposbase/3_0_0")]
    public partial class id
    {

        private string idEsquemaField;
        private string organizacionIdEsquemaField;
        private string valueField;

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "token")]
        [JsonProperty("@idEsquema", NullValueHandling = NullValueHandling.Ignore)]
        public string idEsquema
        {
            get
            {
                return this.idEsquemaField;
            }
            set
            {
                this.idEsquemaField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "token")]
        [JsonProperty("@organizacionIdEsquema", NullValueHandling = NullValueHandling.Ignore)]
        public string organizacionIdEsquema
        {
            get
            {
                return this.organizacionIdEsquemaField;
            }
            set
            {
                this.organizacionIdEsquemaField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlTextAttribute()]
        [JsonProperty("$")]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("svcutil", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ws.bancogalicia.com.ar/webservices/globales/bgbaheader/2_0_0")]
    public partial class organizacionInterna
    {
        private string tipoField;

        private ulong idField;

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]

        [JsonProperty("@tipo")]
        public string tipo
        {
            get
            {
                return this.tipoField;
            }
            set
            {
                this.tipoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [JsonProperty("@id")]
        public ulong id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("svcutil", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ws.bancogalicia.com.ar/webservices/globales/bgbaheader/2_0_0")]
    public partial class Origen
    {

        private ModuloAplicativo moduloAplicativoField;

        private string canalField;

        private organizacionInterna organizacionInternaField;

        private equipo equipoField;

        private string terminalField;

        private System.DateTime fechaHoraCreacionField;

        private bool fechaHoraCreacionFieldSpecified;

        private IdCliente idClienteField;

        private Operador operadorField;

        private string supervisionField;

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        [JsonProperty("_01:ModuloAplicativo")]
        public ModuloAplicativo ModuloAplicativo
        {
            get
            {
                return this.moduloAplicativoField;
            }
            set
            {
                this.moduloAplicativoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        [JsonProperty("_01:Canal")]
        public string Canal
        {
            get
            {
                return this.canalField;
            }
            set
            {
                this.canalField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        [JsonProperty("_01:OrganizacionInterna")]
        public organizacionInterna OrganizacionInterna
        {
            get
            {
                return this.organizacionInternaField;
            }
            set
            {
                this.organizacionInternaField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 3)]
        [JsonProperty("_01:Equipo", NullValueHandling = NullValueHandling.Ignore)]
        public equipo Equipo
        {
            get
            {
                return this.equipoField;
            }
            set
            {
                this.equipoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 4)]
        [JsonProperty("_01:Terminal")]
        public string Terminal
        {
            get
            {
                return this.terminalField;
            }
            set
            {
                this.terminalField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 5)]
        [JsonIgnore]
        public System.DateTime FechaHoraCreacion
        {
            get
            {
                return this.fechaHoraCreacionField;
            }
            set
            {
                this.fechaHoraCreacionField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [JsonIgnore]
        public bool FechaHoraCreacionSpecified
        {
            get
            {
                return this.fechaHoraCreacionFieldSpecified;
            }
            set
            {
                this.fechaHoraCreacionFieldSpecified = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 6)]
        [JsonProperty("_01:IdCliente")]
        public IdCliente IdCliente
        {
            get
            {
                return this.idClienteField;
            }
            set
            {
                this.idClienteField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 7)]
        [JsonProperty("_01:Operador")]
        public Operador Operador
        {
            get
            {
                return this.operadorField;
            }
            set
            {
                this.operadorField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 8)]
        [JsonProperty("_01:Supervision")]
        public string Supervision
        {
            get
            {
                return this.supervisionField;
            }
            set
            {
                this.supervisionField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("svcutil", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://ws.bancogalicia.com.ar/webservices/globales/bgbaheader/2_0_0")]
    public partial class ModuloAplicativo
    {

        private string idGaliciaField;

        private string idConsumidorField;

        private string idProveedorField;

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        [JsonProperty("_01:IdGalicia")]
        public string IdGalicia
        {
            get
            {
                return this.idGaliciaField;
            }
            set
            {
                this.idGaliciaField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 1)]
        [JsonProperty("_01:IdConsumidor")]
        public string IdConsumidor
        {
            get
            {
                return this.idConsumidorField;
            }
            set
            {
                this.idConsumidorField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 2)]
        [JsonProperty("_01:IdProveedor")]
        public string IdProveedor
        {
            get
            {
                return this.idProveedorField;
            }
            set
            {
                this.idProveedorField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("svcutil", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://ws.bancogalicia.com.ar/webservices/globales/bgbaheader/2_0_0")]
    public partial class equipo
    {

        private string ipField;

        private string nombreField;

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [JsonProperty("@ip")]
        public string ip
        {
            get
            {
                return this.ipField;
            }
            set
            {
                this.ipField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [JsonProperty("@nombre")]
        public string nombre
        {
            get
            {
                return this.nombreField;
            }
            set
            {
                this.nombreField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("svcutil", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://ws.bancogalicia.com.ar/webservices/globales/bgbaheader/2_0_0")]
    public partial class IdCliente
    {

        private string idEsquemaField;

        private string valueField;

        public IdCliente()
        {
            this.idEsquemaField = "NV";
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute("NV")]
        [JsonProperty("@idEsquema")]
        public string idEsquema
        {
            get
            {
                return this.idEsquemaField;
            }
            set
            {
                this.idEsquemaField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlTextAttribute()]
        [JsonProperty("$")]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("svcutil", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://ws.bancogalicia.com.ar/webservices/globales/bgbaheader/2_0_0")]
    public partial class Operador
    {

        private string idEsquemaField;

        private string valueField;

        public Operador()
        {
            this.idEsquemaField = "LEGAJO";
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [System.ComponentModel.DefaultValueAttribute("LEGAJO")]
        [JsonProperty("@idEsquema")]
        public string idEsquema
        {
            get
            {
                return this.idEsquemaField;
            }
            set
            {
                this.idEsquemaField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlTextAttribute()]
        [JsonProperty("$")]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("svcutil", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://ws.bancogalicia.com.ar/webservices/accionesseguridadomnichannel/generarsem" +
        "illarequest/1_0_0")]
    public partial class GenerarSemillaRequestDatos
    {
        private id idClienteField;

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute(Order = 0)]
        [JsonProperty("_0:idCliente")]
        public id idCliente
        {
            get
            {
                return this.idClienteField;
            }
            set
            {
                this.idClienteField = value;
            }
        }
    }
}
