using System.Text;
using DevBase.Net.Configuration;
using DevBase.Net.Parsing;

namespace DevBase.Test.DevBaseRequests;

[TestFixture]
public class ResponseMultiSelectorTest
{
    private const string TestJson = @"{
        ""user"": {
            ""id"": 123,
            ""name"": ""John Doe"",
            ""email"": ""john@example.com"",
            ""age"": 30,
            ""isActive"": true,
            ""balance"": 1234.56,
            ""address"": {
                ""city"": ""New York"",
                ""zip"": ""10001"",
                ""country"": ""USA""
            }
        },
        ""product"": {
            ""id"": 456,
            ""title"": ""Widget"",
            ""price"": 99.99,
            ""inStock"": true
        },
        ""metadata"": {
            ""timestamp"": 1234567890,
            ""version"": ""1.0.0""
        }
    }";

    [Test]
    public void Parse_WithSelectors_ExtractsAllValues()
    {
        var parser = new MultiSelectorParser();
        byte[] json = Encoding.UTF8.GetBytes(TestJson);

        var result = parser.Parse(json,
            ("userId", "$.user.id"),
            ("userName", "$.user.name"),
            ("userEmail", "$.user.email")
        );

        Assert.That(result.GetInt("userId"), Is.EqualTo(123));
        Assert.That(result.GetString("userName"), Is.EqualTo("John Doe"));
        Assert.That(result.GetString("userEmail"), Is.EqualTo("john@example.com"));
    }

    [Test]
    public void Parse_WithConfig_ExtractsAllValues()
    {
        var parser = new MultiSelectorParser();
        byte[] json = Encoding.UTF8.GetBytes(TestJson);

        var config = MultiSelectorConfig.Create(
            ("userId", "$.user.id"),
            ("productId", "$.product.id")
        );

        var result = parser.Parse(json, config);

        Assert.That(result.GetInt("userId"), Is.EqualTo(123));
        Assert.That(result.GetInt("productId"), Is.EqualTo(456));
    }

    [Test]
    public void ParseOptimized_WithCommonPrefix_ExtractsAllValues()
    {
        var parser = new MultiSelectorParser();
        byte[] json = Encoding.UTF8.GetBytes(TestJson);

        var result = parser.ParseOptimized(json,
            ("id", "$.user.id"),
            ("name", "$.user.name"),
            ("email", "$.user.email"),
            ("age", "$.user.age")
        );

        Assert.That(result.GetInt("id"), Is.EqualTo(123));
        Assert.That(result.GetString("name"), Is.EqualTo("John Doe"));
        Assert.That(result.GetString("email"), Is.EqualTo("john@example.com"));
        Assert.That(result.GetInt("age"), Is.EqualTo(30));
    }

    [Test]
    public void Parse_NestedPaths_ExtractsCorrectly()
    {
        var parser = new MultiSelectorParser();
        byte[] json = Encoding.UTF8.GetBytes(TestJson);

        var result = parser.Parse(json,
            ("city", "$.user.address.city"),
            ("zip", "$.user.address.zip"),
            ("country", "$.user.address.country")
        );

        Assert.That(result.GetString("city"), Is.EqualTo("New York"));
        Assert.That(result.GetString("zip"), Is.EqualTo("10001"));
        Assert.That(result.GetString("country"), Is.EqualTo("USA"));
    }

    [Test]
    public void Parse_DifferentTypes_ExtractsCorrectly()
    {
        var parser = new MultiSelectorParser();
        byte[] json = Encoding.UTF8.GetBytes(TestJson);

        var result = parser.Parse(json,
            ("userId", "$.user.id"),
            ("userName", "$.user.name"),
            ("userAge", "$.user.age"),
            ("isActive", "$.user.isActive"),
            ("balance", "$.user.balance")
        );

        Assert.That(result.GetInt("userId"), Is.EqualTo(123));
        Assert.That(result.GetString("userName"), Is.EqualTo("John Doe"));
        Assert.That(result.GetInt("userAge"), Is.EqualTo(30));
        Assert.That(result.GetBool("isActive"), Is.EqualTo(true));
        Assert.That(result.GetDouble("balance"), Is.EqualTo(1234.56).Within(0.001));
    }

    [Test]
    public void Parse_MixedSections_ExtractsAllValues()
    {
        var parser = new MultiSelectorParser();
        byte[] json = Encoding.UTF8.GetBytes(TestJson);

        var result = parser.Parse(json,
            ("userId", "$.user.id"),
            ("productId", "$.product.id"),
            ("userName", "$.user.name"),
            ("productTitle", "$.product.title"),
            ("timestamp", "$.metadata.timestamp")
        );

        Assert.That(result.GetInt("userId"), Is.EqualTo(123));
        Assert.That(result.GetInt("productId"), Is.EqualTo(456));
        Assert.That(result.GetString("userName"), Is.EqualTo("John Doe"));
        Assert.That(result.GetString("productTitle"), Is.EqualTo("Widget"));
        Assert.That(result.GetLong("timestamp"), Is.EqualTo(1234567890));
    }

    [Test]
    public void Parse_NonExistentPath_ReturnsNullForMissing()
    {
        var parser = new MultiSelectorParser();
        byte[] json = Encoding.UTF8.GetBytes(TestJson);

        var result = parser.Parse(json,
            ("userId", "$.user.id"),
            ("missing", "$.user.nonexistent")
        );

        Assert.That(result.GetInt("userId"), Is.EqualTo(123));
        Assert.That(result.HasValue("missing"), Is.False);
        Assert.That(result.GetString("missing"), Is.Null);
    }

    [Test]
    public void ParseOptimized_ConfigWithOptimization_ExtractsCorrectly()
    {
        var parser = new MultiSelectorParser();
        byte[] json = Encoding.UTF8.GetBytes(TestJson);

        var config = MultiSelectorConfig.CreateOptimized(
            ("id", "$.user.id"),
            ("name", "$.user.name"),
            ("email", "$.user.email")
        );

        var result = parser.Parse(json, config);

        Assert.That(config.OptimizePathReuse, Is.True);
        Assert.That(result.GetInt("id"), Is.EqualTo(123));
        Assert.That(result.GetString("name"), Is.EqualTo("John Doe"));
        Assert.That(result.GetString("email"), Is.EqualTo("john@example.com"));
    }

    [Test]
    public void Parse_EmptySelectors_ReturnsEmptyResult()
    {
        var parser = new MultiSelectorParser();
        byte[] json = Encoding.UTF8.GetBytes(TestJson);

        var result = parser.Parse(json);

        Assert.That(result.Count, Is.EqualTo(0));
    }

    [Test]
    public void Parse_ResultNames_ContainsAllSelectors()
    {
        var parser = new MultiSelectorParser();
        byte[] json = Encoding.UTF8.GetBytes(TestJson);

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
