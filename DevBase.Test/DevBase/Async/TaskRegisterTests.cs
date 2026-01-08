using DevBase.Async.Task;

namespace DevBase.Test.DevBase.Async;

/// <summary>
/// Tests for the TaskRegister class.
/// Verifies task registration, suspension, resumption, and token management.
/// </summary>
public class TaskRegisterTests
{
    /// <summary>
    /// Tests that GenerateNewToken creates a valid token for a type.
    /// </summary>
    [Test]
    public void GenerateNewToken_CreatesValidToken()
    {
        var register = new TaskRegister();
        
        var token = register.GenerateNewToken("TestType");
        
        Assert.That(token, Is.Not.Null);
        Assert.That(token, Is.InstanceOf<TaskSuspensionToken>());
    }

    /// <summary>
    /// Tests that GenerateNewToken returns the same token for the same type.
    /// </summary>
    [Test]
    public void GenerateNewToken_ReturnsSameTokenForSameType()
    {
        var register = new TaskRegister();
        
        var token1 = register.GenerateNewToken("TestType");
        var token2 = register.GenerateNewToken("TestType");
        
        Assert.That(token1, Is.SameAs(token2));
    }

    /// <summary>
    /// Tests that GenerateNewToken returns different tokens for different types.
    /// </summary>
    [Test]
    public void GenerateNewToken_ReturnsDifferentTokensForDifferentTypes()
    {
        var register = new TaskRegister();
        
        var token1 = register.GenerateNewToken("Type1");
        var token2 = register.GenerateNewToken("Type2");
        
        Assert.That(token1, Is.Not.SameAs(token2));
    }

    /// <summary>
    /// Tests that GetTokenByType returns the correct token.
    /// </summary>
    [Test]
    public void GetTokenByType_ReturnsCorrectToken()
    {
        var register = new TaskRegister();
        var generatedToken = register.GenerateNewToken("TestType");
        
        var retrievedToken = register.GetTokenByType("TestType");
        
        Assert.That(retrievedToken, Is.SameAs(generatedToken));
    }

    /// <summary>
    /// Tests that Suspend suspends the token for a type.
    /// </summary>
    [Test]
    public void Suspend_SuspendsToken()
    {
        var register = new TaskRegister();
        var token = register.GenerateNewToken("TestType");
        
        register.Suspend("TestType");
        
        Assert.That(token.IsSuspended, Is.True);
    }

    /// <summary>
    /// Tests that Resume resumes a suspended token.
    /// </summary>
    [Test]
    public void Resume_ResumesToken()
    {
        var register = new TaskRegister();
        var token = register.GenerateNewToken("TestType");
        
        register.Suspend("TestType");
        Assert.That(token.IsSuspended, Is.True);
        
        register.Resume("TestType");
        Assert.That(token.IsSuspended, Is.False);
    }

    /// <summary>
    /// Tests that Suspend with multiple types suspends all of them.
    /// </summary>
    [Test]
    public void Suspend_MultipleTypes_SuspendsAll()
    {
        var register = new TaskRegister();
        var token1 = register.GenerateNewToken("Type1");
        var token2 = register.GenerateNewToken("Type2");
        var token3 = register.GenerateNewToken("Type3");
        
        register.Suspend("Type1", "Type2", "Type3");
        
        Assert.That(token1.IsSuspended, Is.True);
        Assert.That(token2.IsSuspended, Is.True);
        Assert.That(token3.IsSuspended, Is.True);
    }

    /// <summary>
    /// Tests that RegisterTask with action creates and starts a task.
    /// </summary>
    [Test]
    public async Task RegisterTask_WithAction_CreatesAndStartsTask()
    {
        var register = new TaskRegister();
        var executed = false;
        
        register.RegisterTask(() => { executed = true; }, "TestTask", startAfterCreation: true);
        
        // Wait for task to execute
        await Task.Delay(100);
        
        Assert.That(executed, Is.True);
    }

    /// <summary>
    /// Tests that RegisterTask with startAfterCreation=false does not start immediately.
    /// </summary>
    [Test]
    public async Task RegisterTask_WithStartAfterCreationFalse_DoesNotStartImmediately()
    {
        var register = new TaskRegister();
        var executed = false;
        var task = new Task(() => { executed = true; });
        
        register.RegisterTask(task, "TestTask", startAfterCreation: false);
        
        await Task.Delay(100);
        
        Assert.That(executed, Is.False);
    }

    /// <summary>
    /// Tests that tokens work with integer type identifiers.
    /// </summary>
    [Test]
    public void GenerateNewToken_WorksWithIntegerTypes()
    {
        var register = new TaskRegister();
        
        var token1 = register.GenerateNewToken(1);
        var token2 = register.GenerateNewToken(2);
        var token1Again = register.GenerateNewToken(1);
        
        Assert.That(token1, Is.Not.SameAs(token2));
        Assert.That(token1, Is.SameAs(token1Again));
    }

    /// <summary>
    /// Tests that tokens work with custom object type identifiers.
    /// </summary>
    [Test]
    public void GenerateNewToken_WorksWithCustomObjects()
    {
        var register = new TaskRegister();
        var key1 = new TestKey("key1");
        var key2 = new TestKey("key2");
        var key1Duplicate = new TestKey("key1");
        
        var token1 = register.GenerateNewToken(key1);
        var token2 = register.GenerateNewToken(key2);
        var token1Again = register.GenerateNewToken(key1Duplicate);
        
        Assert.That(token1, Is.Not.SameAs(token2));
        Assert.That(token1, Is.SameAs(token1Again)); // Same because Equals returns true
    }

    private class TestKey
    {
        private readonly string _value;

        public TestKey(string value)
        {
            _value = value;
        }

        public override bool Equals(object? obj)
        {
            return obj is TestKey other && _value == other._value;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
    }
}
