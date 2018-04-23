namespace Services.N.Trace
{
    public class TraceServiceBase 
    {
        public event TraceEventHandler TraceHandler;
        public bool ForceDebug { get; set; }

        public void Communicator_TraceHandler(object sender, TraceEventArgs ea)
        {
            ea.ForceDebug = ForceDebug;

            if (this.TraceHandler != null)
                this.TraceHandler(sender, ea);
        }
    }
}
