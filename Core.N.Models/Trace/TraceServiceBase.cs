namespace Models.N.Core.Trace
{
    public interface ITraceServiceBase 
    {
        event TraceEventHandler TraceHandler;
        bool ForceDebug { get; set; }

        void Communicator_TraceHandler(object sender, TraceEventArgs ea);

    }
}
