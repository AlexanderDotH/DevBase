using DevBase.Async.Task;

namespace DevBase.Generics;

public class AsyncAList<T> : AList<T>
{
    private readonly Multitasking _multitasking;
    
    public AsyncAList(int concurrentTasks = 2)
    {
        this._multitasking = new Multitasking(concurrentTasks);
    }

    public async Task<T> Find(Predicate<T> predicate, int sliceSize = 10)
    {
        AList<AList<T>> sliced = this.Slice(sliceSize);

        T elementFound = default;
        bool isElementFound = false;

        for (int i = 0; i < sliced.Length; i++)
        {
            AList<T> elements = sliced.Get(i);
            
            this._multitasking.Register(async () =>
            {
                T? result = Array.Find(elements.GetAsArray(), predicate);
                    
                if (result == null)
                    return;

                elementFound = result;
                isElementFound = true;
            });
        }

        while (isElementFound == false)
        {
            await Task.Delay(10);
        }

        this._multitasking.KillAll();
        
        return elementFound;
    }
}