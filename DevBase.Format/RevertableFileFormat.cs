namespace DevBase.Format.Formats;

public abstract class RevertableFileFormat<F, T> : FileFormat<F, T>
{
    public abstract F Revert(T to);
}