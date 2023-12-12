namespace DevBase.Format;

public abstract class FileFormat
{
    public abstract C ParseTo<F, C>(F from);
}