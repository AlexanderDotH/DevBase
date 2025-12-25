using DevBase.Net.Configuration;
using DevBase.Net.Configuration.Enums;
using NUnit.Framework;

namespace DevBase.Test.DevBaseRequests;

/// <summary>
/// Tests for retry policy behavior.
/// All errors (timeout, network, proxy, rate limit) count as attempts.
/// </summary>
[TestFixture]
public class RateLimitRetryTest
{
    #region RetryPolicy Configuration Tests

    [Test]
    public void RetryPolicy_Default_HasThreeRetries()
    {
        var policy = RetryPolicy.Default;
        
        Assert.That(policy.MaxRetries, Is.EqualTo(3));
    }

    [Test]
    public void RetryPolicy_None_HasZeroRetries()
    {
        var policy = RetryPolicy.None;
        
        Assert.That(policy.MaxRetries, Is.EqualTo(0));
    }

    [Test]
    public void RetryPolicy_Aggressive_HasFiveRetries()
    {
        var policy = RetryPolicy.Aggressive;
        
        Assert.That(policy.MaxRetries, Is.EqualTo(5));
    }

    [Test]
    public void RetryPolicy_CanSetMaxRetries()
    {
        var policy = new RetryPolicy
        {
            MaxRetries = 10
        };
        
        Assert.That(policy.MaxRetries, Is.EqualTo(10));
    }

    [Test]
    public void RetryPolicy_ZeroRetriesMeansNoRetry()
    {
        var policy = new RetryPolicy
        {
            MaxRetries = 0
        };
        
        Assert.That(policy.MaxRetries, Is.EqualTo(0));
    }

    #endregion

    #region Backoff Strategy Tests

    [Test]
    public void RetryPolicy_GetDelay_ZeroAttempt_ReturnsZero()
    {
        var policy = new RetryPolicy
        {
            InitialDelay = TimeSpan.FromSeconds(1)
        };
        
        Assert.That(policy.GetDelay(0), Is.EqualTo(TimeSpan.Zero));
    }

    [Test]
    public void RetryPolicy_GetDelay_FixedStrategy_ReturnsSameDelay()
    {
        var policy = new RetryPolicy
        {
            BackoffStrategy = EnumBackoffStrategy.Fixed,
            InitialDelay = TimeSpan.FromSeconds(1)
        };
        
        Assert.That(policy.GetDelay(1), Is.EqualTo(TimeSpan.FromSeconds(1)));
        Assert.That(policy.GetDelay(2), Is.EqualTo(TimeSpan.FromSeconds(1)));
        Assert.That(policy.GetDelay(3), Is.EqualTo(TimeSpan.FromSeconds(1)));
    }

    [Test]
    public void RetryPolicy_GetDelay_LinearStrategy_ReturnsLinearDelay()
    {
        var policy = new RetryPolicy
        {
            BackoffStrategy = EnumBackoffStrategy.Linear,
            InitialDelay = TimeSpan.FromSeconds(1),
            MaxDelay = TimeSpan.FromSeconds(100)
        };
        
        Assert.That(policy.GetDelay(1), Is.EqualTo(TimeSpan.FromSeconds(1)));
        Assert.That(policy.GetDelay(2), Is.EqualTo(TimeSpan.FromSeconds(2)));
        Assert.That(policy.GetDelay(3), Is.EqualTo(TimeSpan.FromSeconds(3)));
    }

    [Test]
    public void RetryPolicy_GetDelay_ExponentialStrategy_ReturnsExponentialDelay()
    {
        var policy = new RetryPolicy
        {
            BackoffStrategy = EnumBackoffStrategy.Exponential,
            InitialDelay = TimeSpan.FromSeconds(1),
            BackoffMultiplier = 2.0,
            MaxDelay = TimeSpan.FromSeconds(100)
        };
        
        Assert.That(policy.GetDelay(1), Is.EqualTo(TimeSpan.FromSeconds(1)));
        Assert.That(policy.GetDelay(2), Is.EqualTo(TimeSpan.FromSeconds(2)));
        Assert.That(policy.GetDelay(3), Is.EqualTo(TimeSpan.FromSeconds(4)));
    }

    [Test]
    public void RetryPolicy_GetDelay_RespectsMaxDelay()
    {
        var policy = new RetryPolicy
        {
            BackoffStrategy = EnumBackoffStrategy.Exponential,
            InitialDelay = TimeSpan.FromSeconds(10),
            BackoffMultiplier = 10.0,
            MaxDelay = TimeSpan.FromSeconds(30)
        };
        
        // 10 * 10^2 = 1000 seconds, but should be capped at 30
        Assert.That(policy.GetDelay(3), Is.EqualTo(TimeSpan.FromSeconds(30)));
    }

    #endregion

    #region Policy Presets Tests

    [Test]
    public void RetryPolicy_Default_HasCorrectSettings()
    {
        var policy = RetryPolicy.Default;
        
        Assert.That(policy.MaxRetries, Is.EqualTo(3));
        Assert.That(policy.BackoffStrategy, Is.EqualTo(EnumBackoffStrategy.Exponential));
    }

    [Test]
    public void RetryPolicy_None_HasZeroRetriesPreset()
    {
        var policy = RetryPolicy.None;
        
        Assert.That(policy.MaxRetries, Is.EqualTo(0));
    }

    [Test]
    public void RetryPolicy_Aggressive_HasMoreRetries()
    {
        var policy = RetryPolicy.Aggressive;
        
        Assert.That(policy.MaxRetries, Is.EqualTo(5));
        Assert.That(policy.InitialDelay, Is.EqualTo(TimeSpan.FromMilliseconds(100)));
        Assert.That(policy.MaxDelay, Is.EqualTo(TimeSpan.FromSeconds(10)));
    }

    #endregion
}
