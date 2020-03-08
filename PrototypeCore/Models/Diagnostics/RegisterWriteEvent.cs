namespace PrototypeCore.Models.Diagnostics
{
    public class RegisterWriteEvent : BaseDiagnosticsEvent
    {
        public string RegisterType { get; set; }

        public int OldValue { get; set; }
        
        public int NewValue { get; set; }
    }
}
