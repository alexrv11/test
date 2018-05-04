﻿using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace BGBA.MS.N.Consulta.Cliente
{
    public class TelemetryInitializer : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            var requestTelemetry = telemetry as RequestTelemetry;

            if (requestTelemetry == null) return;

            requestTelemetry.Context.Properties["cloud_RoleName"] = "MS.N.Generar.CUIx-ROLE";
        }
    }
}
