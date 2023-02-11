namespace Shared;

public record JourneyMessage(Guid Id, string Username, DateTime CreatedAt);
public record JourneyRequest(string Username);