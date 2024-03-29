﻿using System.Diagnostics;
using DevBase.Requests.Abstract;
using DevBase.Requests.Exceptions;
using DevBase.Requests.Preparation.Header.Body;
using DevBase.Requests.Preparation.Header.UserAgent;
using Dumpify;

namespace DevBase.Test.DevBaseRequests.Builder;

public class UserAgentBuilderTest
{
    [Test]
    public void BuildBogusUserAgentTest()
    {
        UserAgentHeaderBuilder builder = new UserAgentHeaderBuilder().BuildBogus();

        Assert.NotNull(builder.UserAgent.ToString());
        
        Console.WriteLine($"Generated random user-agent: {builder.UserAgent}");
    }
    
    [Test]
    public void BuildCustomUserAgentTest()
    {
        UserAgentHeaderBuilder builder = new UserAgentHeaderBuilder()
            .AddProductName("Microsoft Excel ;)")
            .AddProductVersion("1.0")
            .Build();

        Assert.NotNull(builder.UserAgent.ToString());
        
        Console.WriteLine($"Built user-agent: {builder.UserAgent}");
    }
    
    [Test]
    public void BuildCustomBogusUserAgentTest()
    {
        UserAgentHeaderBuilder builder = new UserAgentHeaderBuilder()
            .AddProductName("Microsoft Excel ;)")
            .AddProductVersion("1.0")
            .Build()
            .BuildBogus();

        Assert.NotNull(builder.UserAgent.ToString());
        
        Console.WriteLine($"Built user-agent: {builder.UserAgent}");
    }
    
    [Test]
    public void BuildCustomBogusCustomUserAgentTest()
    {
        Assert.Throws<HttpHeaderException>(() =>
        {
            new UserAgentHeaderBuilder()
                .AddProductName("Microsoft Excel ;)")
                .AddProductVersion("1.0")
                .Build()
                .BuildBogus()
                .Build();
        });
        
        Console.WriteLine($"It should throw an \"HttpHeaderException\" and it worked!");
    }
}