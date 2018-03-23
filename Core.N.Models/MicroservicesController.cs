using Microsoft.AspNetCore.Mvc;

namespace Core.N.Models
{
    public class MicroservicesController : Controller
    {
        //protected void Communicator_TraceHandler(object sender, Trace.TraceEventArgs ea, BGBA.HB.Models.Users.User currentUser, BGBA.HB.Log.UserLogInfo userLogInfo, string descripcion)
        //{
        //    //WARNING - NO ACCEDER AL OBJETO SESSION ACA. CUANDO FINALIZA EL TREAD DEL USUARIO EL OBJETO SESSION SE NULEA.

        //    bool forceLog = false;
        //    //para el caso de que se fuerze el debug en el login el user log info es null y solo registrariamos con ip
        //    if (userLogInfo == null)
        //    {
        //        userLogInfo = new Log.UserLogInfo();
        //        userLogInfo.IP = GetCurrentClientIP();
        //        forceLog = true;
        //    }

        //    if (ea.IsError)
        //    {
        //        LogServices.Log.Error(userLogInfo, new BGBA.HB.Log.LogInfo()
        //        {
        //            Clase = this.GetType().Name,
        //            Metodo = System.Reflection.MethodBase.GetCurrentMethod().Name,
        //            Pagina = ea.URI + " | " + Helpers.ApplicationHelper.GetCurrentPage(),
        //            ModuloID = Helpers.ApplicationHelper.GetCurrentModuleCode(),
        //            Evento = Log.EventoLog.Trace,
        //            IdentificadorInternoOperacion = (Session != null) ? Session.SessionID : String.Empty,
        //            Categoria = Log.LogInfo.CategoriaLog.Technical
        //        }, new Log.LogInfoDetalle()
        //        {
        //            //Destino = ea.URI,
        //            DetalleInput = ea.Request,
        //            DetalleOutput = ea.Response,
        //            Servicio = ea.URI,
        //            TiempoTranscurrido = ea.ElapsedTime
        //        }, descripcion);

        //        return;
        //    }

        //    var logInfoDetalle = new Log.LogInfoDetalle()
        //    {
        //        Servicio = ea.URI,
        //        TiempoTranscurrido = ea.ElapsedTime
        //    };

        //    //Para que logee TRACE el usuario tiene que tener habilitada la marca de debug o la aplicación debe tener habilitada la marca de debug...
        //    if ((userLogInfo != null && userLogInfo.DebugActivo) || forceLog)
        //    {
        //        logInfoDetalle.DetalleInput = ea.Request;
        //        logInfoDetalle.DetalleOutput = ea.Response;
        //    }

        //    LogServices.Log.Debug(userLogInfo, new BGBA.HB.Log.LogInfo()
        //    {
        //        Clase = this.GetType().Name,
        //        Metodo = System.Reflection.MethodBase.GetCurrentMethod().Name,
        //        Pagina = ea.URI + " | " + Helpers.ApplicationHelper.GetCurrentPage(),
        //        ModuloID = GetCurrentModule().Code.ToString(),
        //        Evento = Log.EventoLog.Trace,
        //        IdentificadorInternoOperacion = (Session != null) ? Session.SessionID : String.Empty,
        //    }, logInfoDetalle, descripcion);
        //}
    }
}
