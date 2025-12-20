using DevBase.Async.Task;

namespace DevBase.Test.DevBase;

public class MultitaskingTest
{
    [Test]
    public async Task MultitaskingRegisterAndWaitTest()
    {
        Multitasking multitasking = new Multitasking(2);

        for (int i = 0; i < 200; i++)
        {
            multitasking.Register(async () =>
            {
                await Task.Delay(1000);
                
                Console.WriteLine(i);
            });
        }

        await multitasking.WaitAll();
        
        Console.WriteLine("Finished");
    }
}
