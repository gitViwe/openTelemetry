using Microsoft.AspNetCore.Hosting;
using System.Text.Json;

namespace Shared;

public class SuperHeroData
{
    private const string HERO_JSON_FILE = "superheroes.json";
    private readonly IWebHostEnvironment _environment;

    public SuperHeroData(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<IEnumerable<SuperHeroResponse>> GetEnumerableAsync(CancellationToken token = default)
    {
        var filePath = Path.Combine(_environment.WebRootPath, HERO_JSON_FILE);
        var jsonString = await File.ReadAllTextAsync(filePath, token);
        return JsonSerializer.Deserialize<IEnumerable<SuperHeroResponse>>(jsonString)
            ?? Enumerable.Empty<SuperHeroResponse>();
    }
}
