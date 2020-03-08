using PrototypeCore.Interfaces.Diagnostics;
using System;

namespace PrototypeCore.Models.Diagnostics
{
    public abstract class BaseDiagnosticsEvent : IDiagnosticsEvent
    {
        public DateTime DateTime { get; } = DateTime.UtcNow;

        public string TypeDescription => this.GetType().Name;
    }
}
