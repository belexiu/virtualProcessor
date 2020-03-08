using System.Collections.Generic;

namespace PrototypeCore.Interfaces.Diagnostics
{
    public interface IDiagnosticsService
    {
        void AddEvent(IDiagnosticsEvent diagnosticsEvent);

        List<IDiagnosticsEvent> GetAllEvents();
    }
}
