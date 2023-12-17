namespace DevBase.Typography.Encoded;

public abstract class EncodedAString : AString
{
    public abstract AString GetDecoded();

    public abstract bool IsEncoded();
    
    protected EncodedAString(string value) : base(value) { }
}