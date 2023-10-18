using System.Collections.Generic;

namespace IntegrationTests.Model;

public record ErrorResponse(string Title, Dictionary<string, string[]>? Errors);