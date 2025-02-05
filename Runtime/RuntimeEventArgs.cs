public class RuntimeEventArgs : EventArgs
{
    public string EventType { get; set; }
    public string Message { get; set; }
    public object Data { get; set; }
}
