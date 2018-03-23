using System;
using System.Collections.Generic;
using System.Text;

namespace Core.N.Trace
{
    public class TraceEventArgs
    {
        public int ElapsedTime { set; get; }
        public bool IsError { set; get; }
        public string URL { set; get; }
        public string Request { set; get; }
        public string Response { set; get; }
        public bool ForceDebug { get; set; }
    }
}
