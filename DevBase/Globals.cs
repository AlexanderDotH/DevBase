namespace DevBase;

public class Globals
{
    /// <summary>
    /// Controls whether BinaryFormatter serialization is allowed for size measurement.
    /// Disabled by default for .NET 9+ compatibility where BinaryFormatter is removed.
    /// When disabled, object size estimation uses reflection-based calculation.
    /// </summary>
    public static bool AllowSerialization { get; set; } = false;
}