using DevBase.Net.Abstract;
using DevBase.Net.Exceptions;

namespace DevBase.Net.Render;

public static class RequestBodyRenderer
{
    public static Memory<byte> RenderKeyValueList<T, K, V>(HttpKeyValueListBuilder<T, K, V> request) 
        where T : HttpKeyValueListBuilder<T, K, V> 
        where K : class 
        where V : class
    {
        if (request == null)
            throw new BuilderException();

        request.Build();
        return request.Buffer;
    }
    
    public static Memory<byte> RenderBody<T>(HttpBodyBuilder<T> request) 
        where T : HttpBodyBuilder<T> 
    {
        if (request == null)
            throw new BuilderException();
        
        request.Build();
        return request.Buffer;
    }
}

