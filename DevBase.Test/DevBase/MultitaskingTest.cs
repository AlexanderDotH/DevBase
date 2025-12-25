using DevBase.Async.Task;

namespace DevBase.Test.DevBase;

/// <summary>
/// Tests for the Multitasking system.
/// </summary>
public class MultitaskingTest
{
    /// <summary>
    /// Tests task registration and waiting mechanism in Multitasking.
    /// Creates 200 tasks with a capacity of 2 and waits for all to complete.
    /// </summary>
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
