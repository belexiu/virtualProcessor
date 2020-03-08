using System;

namespace PrototypeCore.Interfaces.Diagnostics
{
    public interface IDiagnosticsEvent
    {
        public DateTime DateTime { get; }

        public string TypeDescription { get;  }
    }
}
