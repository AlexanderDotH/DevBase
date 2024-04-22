namespace DevBase.Format;

public abstract class RevertableFileFormat<F, T> : FileFormat<F, T>
{
    public abstract F Revert(T to);

    public abstract bool TryRevert(T to, out F from);

}