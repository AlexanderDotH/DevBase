using System.Text;
using DevBase.Net.Configuration;
using DevBase.Net.Parsing;

namespace DevBase.Test.DevBaseRequests;

[TestFixture]
public class MultiSelectorParserTest
{
    private const string SimpleJson = @"{
        ""user"": {
            ""id"": 123,
            ""name"": ""John Doe"",
            ""email"": ""john@example.com"",
            ""age"": 30,
            ""isActive"": true,
            ""balance"": 1234.56,
            ""address"": {
                ""city"": ""New York"",
                ""zip"": ""10001""
            }
        },
        ""product"": {
            ""id"": 456,
            ""title"": ""Widget"",
            ""price"": 99.99
        }
    }";

    private const string ArrayJson = @"{
        ""users"": [
            { ""id"": 1, ""name"": ""Alice"" },
            { ""id"": 2, ""name"": ""Bob"" },
            { ""id"": 3, ""name"": ""Charlie"" }
        ]
    }";

    [Test]
    public void Parse_SingleSelector_ExtractsValue()
    {
        var parser = new MultiSelectorParser();
        byte[] json = Encoding.UTF8.GetBytes(SimpleJson);

        var result = parser.Parse(json, ("userId", "$.user.id"));

        Assert.That(result.HasValue("userId"), Is.True);
        Assert.That(result.GetInt("userId"), Is.EqualTo(123));
    }

    [Test]
    public void Parse_MultipleSelectors_ExtractsAllValues()
    {
        var parser = new MultiSelectorParser();
        byte[] json = Encoding.UTF8.GetBytes(SimpleJson);

        var result = parser.Parse(json,
            ("userId", "$.user.id"),
            ("userName", "$.user.name"),
            ("userEmail", "$.user.email")
        );

        Assert.That(result.HasValue("userId"), Is.True);
        Assert.That(result.HasValue("userName"), Is.True);
        Assert.That(result.HasValue("userEmail"), Is.True);
        
        Assert.That(result.GetInt("userId"), Is.EqualTo(123));
        Assert.That(result.GetString("userName"), Is.EqualTo("John Doe"));
        Assert.That(result.GetString("userEmail"), Is.EqualTo("john@example.com"));
    }

    [Test]
    public void Parse_NestedPaths_ExtractsCorrectly()
    {
        var parser = new MultiSelectorParser();
        byte[] json = Encoding.UTF8.GetBytes(SimpleJson);

        var result = parser.Parse(json,
            ("city", "$.user.address.city"),
            ("zip", "$.user.address.zip")
        );

        Assert.That(result.GetString("city"), Is.EqualTo("New York"));
        Assert.That(result.GetString("zip"), Is.EqualTo("10001"));
    }

    [Test]
    public void Parse_DifferentTypes_ExtractsCorrectly()
    {
        var parser = new MultiSelectorParser();
        byte[] json = Encoding.UTF8.GetBytes(SimpleJson);

        var result = parser.Parse(json,
            ("id", "$.user.id"),
            ("name", "$.user.name"),
            ("age", "$.user.age"),
            ("isActive", "$.user.isActive"),
            ("balance", "$.user.balance")
        );

        Assert.That(result.GetInt("id"), Is.EqualTo(123));
        Assert.That(result.GetString("name"), Is.EqualTo("John Doe"));
        Assert.That(result.GetInt("age"), Is.EqualTo(30));
        Assert.That(result.GetBool("isActive"), Is.EqualTo(true));
        Assert.That(result.GetDouble("balance"), Is.EqualTo(1234.56));
    }

    [Test]
    public void Parse_NonExistentPath_ReturnsNull()
    {
        var parser = new MultiSelectorParser();
        byte[] json = Encoding.UTF8.GetBytes(SimpleJson);

        var result = parser.Parse(json, ("missing", "$.user.nonexistent"));

        Assert.That(result.HasValue("missing"), Is.False);
        Assert.That(result.GetString("missing"), Is.Null);
    }

    [Test]
    public void Parse_CommonPrefix_WithoutOptimization()
    {
        var parser = new MultiSelectorParser();
        byte[] json = Encoding.UTF8.GetBytes(SimpleJson);

        var config = MultiSelectorConfig.Create(
            ("id", "$.user.id"),
            ("name", "$.user.name"),
            ("email", "$.user.email")
        );

        var result = parser.Parse(json, config);

        Assert.That(result.GetInt("id"), Is.EqualTo(123));
        Assert.That(result.GetString("name"), Is.EqualTo("John Doe"));
        Assert.That(result.GetString("email"), Is.EqualTo("john@example.com"));
    }

    [Test]
    public void ParseOptimized_CommonPrefix_WithOptimization()
    {
        var parser = new MultiSelectorParser();
        byte[] json = Encoding.UTF8.GetBytes(SimpleJson);

        var result = parser.ParseOptimized(json,
            ("id", "$.user.id"),
            ("name", "$.user.name"),
            ("email", "$.user.email")
        );

        Assert.That(result.GetInt("id"), Is.EqualTo(123));
        Assert.That(result.GetString("name"), Is.EqualTo("John Doe"));
        Assert.That(result.GetString("email"), Is.EqualTo("john@example.com"));
    }

    [Test]
    public void Parse_MixedPaths_DifferentSections()
    {
        var parser = new MultiSelectorParser();
        byte[] json = Encoding.UTF8.GetBytes(SimpleJson);

        var result = parser.Parse(json,
            ("userId", "$.user.id"),
            ("productId", "$.product.id"),
            ("userName", "$.user.name"),
            ("productTitle", "$.product.title")
        );

        Assert.That(result.GetInt("userId"), Is.EqualTo(123));
        Assert.That(result.GetInt("productId"), Is.EqualTo(456));
        Assert.That(result.GetString("userName"), Is.EqualTo("John Doe"));
        Assert.That(result.GetString("productTitle"), Is.EqualTo("Widget"));
    }

    [Test]
    public void Parse_ArrayPath_ExtractsFirstElement()
    {
        var parser = new MultiSelectorParser();
        byte[] json = Encoding.UTF8.GetBytes(ArrayJson);

        var result = parser.Parse(json,
            ("firstId", "$.users[0].id"),
            ("firstName", "$.users[0].name")
        );

        Assert.That(result.GetInt("firstId"), Is.EqualTo(1));
        Assert.That(result.GetString("firstName"), Is.EqualTo("Alice"));
    }

    [Test]
    public void Parse_EmptySelectors_ReturnsEmptyResult()
    {
        var parser = new MultiSelectorParser();
        byte[] json = Encoding.UTF8.GetBytes(SimpleJson);

        var result = parser.Parse(json);

        Assert.That(result.Count, Is.EqualTo(0));
    }

    [Test]
    public void Parse_ConfigWithOptimization_EnablesPathReuse()
    {
        var parser = new MultiSelectorParser();
        byte[] json = Encoding.UTF8.GetBytes(SimpleJson);

        var config = MultiSelectorConfig.CreateOptimized(
            ("id", "$.user.id"),
            ("name", "$.user.name")
        );

        Assert.That(config.OptimizePathReuse, Is.True);
        Assert.That(config.OptimizeProperties, Is.True);

        var result = parser.Parse(json, config);

        Assert.That(result.GetInt("id"), Is.EqualTo(123));
        Assert.That(result.GetString("name"), Is.EqualTo("John Doe"));
    }

    [Test]
    public void Parse_ConfigWithoutOptimization_DisablesPathReuse()
    {
        var parser = new MultiSelectorParser();
        byte[] json = Encoding.UTF8.GetBytes(SimpleJson);

        var config = MultiSelectorConfig.Create(
            ("id", "$.user.id"),
            ("name", "$.user.name")
        );

        Assert.That(config.OptimizePathReuse, Is.False);
        Assert.That(config.OptimizeProperties, Is.False);

        var result = parser.Parse(json, config);

        Assert.That(result.GetInt("id"), Is.EqualTo(123));
        Assert.That(result.GetString("name"), Is.EqualTo("John Doe"));
    }

    [Test]
    public void Parse_ResultNames_ContainsAllSelectorNames()
    {
        var parser = new MultiSelectorParser();
        byte[] json = Encoding.UTF8.GetBytes(SimpleJson);

        var result = parser.Parse(json,
            ("userId", "$.user.id"),
            ("userName", "$.user.name"),
            ("productId", "$.product.id")
        );

        var names = result.Names.ToList();
        
        Assert.That(names, Contains.Item("userId"));
        Assert.That(names, Contains.Item("userName"));
        Assert.That(names, Contains.Item("productId"));
        Assert.That(names.Count, Is.EqualTo(3));
    }
}
