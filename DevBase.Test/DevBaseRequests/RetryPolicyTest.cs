using DevBase.Requests.Configuration;
using NUnit.Framework;

namespace DevBase.Test.DevBaseRequests;

[TestFixture]
public class RetryPolicyTest
{
    [Test]
    public void Default_HasCorrectSettings()
    {
        var policy = RetryPolicy.Default;
        
        Assert.That(policy.MaxRetries, Is.EqualTo(3));
        Assert.That(policy.BackoffStrategy, Is.EqualTo(BackoffStrategy.Exponential));
    }

    [Test]
    public void None_HasNoRetries()
    {
        var policy = RetryPolicy.None;
        
        Assert.That(policy.MaxRetries, Is.EqualTo(0));
    }

    [Test]
    public void Aggressive_HasMoreRetries()
    {
        var policy = RetryPolicy.Aggressive;
        
        Assert.That(policy.MaxRetries, Is.EqualTo(5));
    }

    [Test]
    public void GetDelay_Exponential_IncreasesExponentially()
    {
        var policy = new RetryPolicy
        {
            MaxRetries = 5,
            BackoffStrategy = BackoffStrategy.Exponential,
            InitialDelay = TimeSpan.FromSeconds(1),
            MaxDelay = TimeSpan.FromMinutes(1)
        };

        var delay1 = policy.GetDelay(1);
        var delay2 = policy.GetDelay(2);
        var delay3 = policy.GetDelay(3);

        Assert.That(delay2, Is.GreaterThan(delay1));
        Assert.That(delay3, Is.GreaterThan(delay2));
    }

    [Test]
    public void GetDelay_Linear_IncreasesLinearly()
    {
        var policy = new RetryPolicy
        {
            MaxRetries = 5,
            BackoffStrategy = BackoffStrategy.Linear,
            InitialDelay = TimeSpan.FromSeconds(1),
            MaxDelay = TimeSpan.FromMinutes(1)
        };

        var delay1 = policy.GetDelay(1);
        var delay2 = policy.GetDelay(2);
        var delay3 = policy.GetDelay(3);

        var diff1 = delay2 - delay1;
        var diff2 = delay3 - delay2;

        Assert.That(diff1.TotalMilliseconds, Is.EqualTo(diff2.TotalMilliseconds).Within(100));
    }

    [Test]
    public void GetDelay_Fixed_ReturnsSameDelay()
    {
        var policy = new RetryPolicy
        {
            MaxRetries = 5,
            BackoffStrategy = BackoffStrategy.Fixed,
            InitialDelay = TimeSpan.FromSeconds(2),
            MaxDelay = TimeSpan.FromMinutes(1)
        };

        var delay1 = policy.GetDelay(1);
        var delay2 = policy.GetDelay(2);
        var delay3 = policy.GetDelay(3);

        Assert.That(delay1, Is.EqualTo(delay2));
        Assert.That(delay2, Is.EqualTo(delay3));
    }

    [Test]
    public void GetDelay_DoesNotExceedMaxDelay()
    {
        var policy = new RetryPolicy
        {
            MaxRetries = 10,
            BackoffStrategy = BackoffStrategy.Exponential,
            InitialDelay = TimeSpan.FromSeconds(1),
            MaxDelay = TimeSpan.FromSeconds(10)
        };

        var delay = policy.GetDelay(10);

        Assert.That(delay, Is.LessThanOrEqualTo(TimeSpan.FromSeconds(10)));
    }

    [Test]
    public void RetryOnTimeout_DefaultTrue()
    {
        var policy = RetryPolicy.Default;
        Assert.That(policy.RetryOnTimeout, Is.True);
    }

    [Test]
    public void RetryOnNetworkError_DefaultTrue()
    {
        var policy = RetryPolicy.Default;
        Assert.That(policy.RetryOnNetworkError, Is.True);
    }

    [Test]
    public void RetryOnProxyError_DefaultTrue()
    {
        var policy = RetryPolicy.Default;
        Assert.That(policy.RetryOnProxyError, Is.True);
    }
}
