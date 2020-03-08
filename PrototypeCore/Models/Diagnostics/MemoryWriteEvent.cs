namespace PrototypeCore.Models.Diagnostics
{
    public class MemoryWriteEvent : BaseDiagnosticsEvent
    {
        public int Location { get; set; }

        public int OldValue { get; set; }

        public int NewValue { get; set; }
    }
}
