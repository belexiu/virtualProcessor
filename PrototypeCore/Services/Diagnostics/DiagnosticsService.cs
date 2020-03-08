using PrototypeCore.Interfaces.Diagnostics;
using System.Collections.Generic;

namespace PrototypeCore.Services.Diagnostics
{
    public class DiagnosticsService : IDiagnosticsService
    {
        List<IDiagnosticsEvent> _events = new List<IDiagnosticsEvent>();

        public void AddEvent(IDiagnosticsEvent diagnosticsEvent)
        {
            _events.Add(diagnosticsEvent);
        }

        public List<IDiagnosticsEvent> GetAllEvents()
        {
            return _events;
        }
    }
}
