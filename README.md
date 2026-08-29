[![](https://img.shields.io/nuget/v/Soenneker.Extensions.Uri.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Extensions.Uri/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.uri/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.uri/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/Soenneker.Extensions.Uri.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Extensions.Uri/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.uri/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.uri/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.Uri
A collection of useful Uri extension methods.

## Installation

```bash
dotnet add package Soenneker.Extensions.Uri
```

## Quick start

```csharp
using Soenneker.Extensions.Uri;

var uri = new Uri("https://example.com");
var result = uri.ReplaceLastSegment(replacement);
```

## Common operations

- `ReplaceLastSegment()` - Returns a new URI in which the last path segment of the specified URI is replaced with the given replacement string.
- `RemoveLastSegment()` - Returns a new absolute URI without its final path segment, while preserving the query and fragment. The original `Uri` is unchanged.
