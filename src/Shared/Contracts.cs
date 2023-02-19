using System.Text.Json.Serialization;

namespace Shared;

public record JourneyMessage(Guid Id, string Username, DateTime CreatedAt);
public record JourneyRequest(string Username);
public class SuperHeroResponse
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("slug")]
    public string Slug { get; set; } = string.Empty;

    [JsonPropertyName("powerstats")]
    public Powerstats Powerstats { get; set; } = new();

    [JsonPropertyName("appearance")]
    public Appearance Appearance { get; set; } = new();

    [JsonPropertyName("biography")]
    public Biography Biography { get; set; } = new();

    [JsonPropertyName("work")]
    public Work Work { get; set; } = new();

    [JsonPropertyName("connections")]
    public Connections Connections { get; set; } = new();

    [JsonPropertyName("images")]
    public Images Images { get; set; } = new();
}

public class Appearance
{
    [JsonPropertyName("gender")]
    public string Gender { get; set; } = string.Empty;

    [JsonPropertyName("race")]
    public string Race { get; set; } = string.Empty;

    [JsonPropertyName("height")]
    public string[] Height { get; set; } = Array.Empty<string>();

    [JsonPropertyName("weight")]
    public string[] Weight { get; set; } = Array.Empty<string>();

    [JsonPropertyName("eyeColor")]
    public string EyeColor { get; set; } = string.Empty;

    [JsonPropertyName("hairColor")]
    public string HairColor { get; set; } = string.Empty;
}

public class Biography
{
    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = string.Empty;

    [JsonPropertyName("alterEgos")]
    public string AlterEgos { get; set; } = string.Empty;

    [JsonPropertyName("aliases")]
    public string[] Aliases { get; set; } = Array.Empty<string>();

    [JsonPropertyName("placeOfBirth")]
    public string PlaceOfBirth { get; set; } = string.Empty;

    [JsonPropertyName("firstAppearance")]
    public string FirstAppearance { get; set; } = string.Empty;

    [JsonPropertyName("publisher")]
    public string Publisher { get; set; } = string.Empty;

    [JsonPropertyName("alignment")]
    public string Alignment { get; set; } = string.Empty;
}

public class Connections
{
    [JsonPropertyName("groupAffiliation")]
    public string GroupAffiliation { get; set; } = string.Empty;

    [JsonPropertyName("relatives")]
    public string Relatives { get; set; } = string.Empty;
}

public class Images
{
    [JsonPropertyName("xs")]
    public Uri Xs { get; set; }

    [JsonPropertyName("sm")]
    public Uri Sm { get; set; }

    [JsonPropertyName("md")]
    public Uri Md { get; set; }

    [JsonPropertyName("lg")]
    public Uri Lg { get; set; }
}

public class Powerstats
{
    [JsonPropertyName("intelligence")]
    public long Intelligence { get; set; }

    [JsonPropertyName("strength")]
    public long Strength { get; set; }

    [JsonPropertyName("speed")]
    public long Speed { get; set; }

    [JsonPropertyName("durability")]
    public long Durability { get; set; }

    [JsonPropertyName("power")]
    public long Power { get; set; }

    [JsonPropertyName("combat")]
    public long Combat { get; set; }
}

public class Work
{
    [JsonPropertyName("occupation")]
    public string Occupation { get; set; } = string.Empty;

    [JsonPropertyName("base")]
    public string Base { get; set; } = string.Empty;
}