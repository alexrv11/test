namespace BGBA.Models.N.Core.Trace
{
    public interface ITraceService
    {
        event TraceEventHandler TraceHandler;
        bool ForceDebug { get; set; }

        void Communicator_TraceHandler(object sender, TraceEventArgs ea);

    }
}
