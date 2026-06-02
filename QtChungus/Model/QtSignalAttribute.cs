public sealed class QtSignalAttribute : Attribute
{
    public string SignalName { get; }
    public string SignalSignature { get; }
    public QtSignalAttribute(string signalName, string signalSignature) => (SignalName, SignalSignature) = (signalName, signalSignature);
}
