[![](https://img.shields.io/nuget/v/Soenneker.Extensions.Uri.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Extensions.Uri/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.uri/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.uri/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/Soenneker.Extensions.Uri.svg?style=for-the-badge)](https://www.nuget.org/packages/Soenneker.Extensions.Uri/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.uri/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.uri/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.Uri
Replace or remove the final path segment of an absolute `Uri` while preserving its query and fragment.

## Installation

```bash
dotnet add package Soenneker.Extensions.Uri
```

## Replace the final segment

```csharp
using Soenneker.Extensions.Uri;

var uri = new Uri("https://example.com/files/old.csv?download=1#preview");
Uri result = uri.ReplaceLastSegment("quarterly report.csv");
// https://example.com/files/quarterly%20report.csv?download=1#preview
```

`ReplaceLastSegment()` treats `replacement` as one raw path segment. Reserved characters such as `/`, `?`, and `#` are percent-encoded rather than allowed to change the URI structure. `.` and `..` are rejected with `UriFormatException` because `System.Uri` canonicalizes them as navigation. Already escaped text is escaped again; pass the unescaped segment value.

If the URI has no explicit path, the method adds one. A trailing slash is treated as the end of the preceding segment.

## Remove the final segment

```csharp
var uri = new Uri("https://example.com/files/report.csv?download=1");
Uri parent = uri.RemoveLastSegment();
// https://example.com/files/?download=1
```

`RemoveLastSegment()` keeps the parent path's trailing slash. A URI with no removable path segment is returned unchanged.

Both methods require an absolute URI, return a new `Uri` when a change is made, and leave the original instance unchanged. They operate on URI syntax only; they do not authorize a destination, prevent SSRF, or verify that the resource exists.
