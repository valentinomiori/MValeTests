using System.Text.Json;

namespace MValeTests.SystemTextJson;

public class SystemTextJsonTest
{
    // [SetUp]
    // public void Setup()
    // {
    // }

    [Test]
    public void TestNullRoundtrip()
    {
        const string json = "null";
        var document = JsonDocument.Parse(json);
        Assert.That(document, Is.Not.Null);
        Assert.That(document.RootElement.ValueKind, Is.EqualTo(JsonValueKind.Null));
        Assert.That(JsonSerializer.Serialize(document), Is.EqualTo(json));
        var element = JsonSerializer.Deserialize<JsonElement>(json);
        Assert.That(element.ValueKind, Is.EqualTo(JsonValueKind.Null));
        Assert.That(JsonSerializer.Serialize(element), Is.EqualTo(json));
    }
}