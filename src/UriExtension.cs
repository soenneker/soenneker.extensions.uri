using System;
using System.Diagnostics.Contracts;

namespace Soenneker.Extensions.Uri;

/// <summary>
/// A collection of useful Uri extension methods
/// </summary>
public static class UriExtension
{
    /// <summary>
    /// Replaces the last path segment of the specified URI with the provided replacement string and returns a new
    /// absolute URI.
    /// </summary>
    /// <remarks>Query and fragment components of the original URI are preserved in the returned URI. Trailing
    /// slashes are not retained after replacement. This method does not modify the original URI instance.</remarks>
    /// <param name="uri">The source URI whose last path segment will be replaced. Must be an absolute URI.</param>
    /// <param name="replacement">The string to use as the new last path segment. Cannot be null.</param>
    /// <returns>A new absolute URI with the last path segment replaced by the specified string. If the URI has no path segment,
    /// the replacement is appended as the first segment.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="uri"/> or <paramref name="replacement"/> is null.</exception>
    [Pure]
    public static System.Uri ReplaceLastSegment(this System.Uri uri, string replacement)
    {
        if (uri is null)
            throw new ArgumentNullException(nameof(uri));
        if (replacement is null)
            throw new ArgumentNullException(nameof(replacement));

        // absoluteUri includes query/fragment
        string s = uri.AbsoluteUri;

        // Find start of ? or #
        int q = s.IndexOf('?');
        int h = s.IndexOf('#');
        int suffixStart;
        if (q < 0)
            suffixStart = h < 0 ? s.Length : h;
        else
            suffixStart = h < 0 ? q : (q < h ? q : h);

        // Work on just the "path-ish" portion [0..suffixStart)
        int end = suffixStart;

        // If it ends with '/', ignore trailing slash for segment replacement.
        bool hadTrailingSlash = end > 0 && s[end - 1] == '/';
        if (hadTrailingSlash)
            end--;

        // Find last slash before the last segment
        int lastSlash = s.LastIndexOf('/', end - 1);
        if (lastSlash < 0)
        {
            // Weird, but fallback: just append
            return new System.Uri(s + "/" + replacement, UriKind.Absolute);
        }

        // If the URI has no segment to replace (e.g., "https://x.com" => lastSlash is in "https://")
        // handle by inserting after authority.
        // For absolute URIs, the first path slash after scheme/authority is "://.../".
        // If lastSlash lands before the path, treat as "no path".
        int schemeSep = s.IndexOf("://", StringComparison.Ordinal);
        if (schemeSep >= 0)
        {
            int authorityStart = schemeSep + 3;
            int firstPathSlash = s.IndexOf('/', authorityStart);
            if (firstPathSlash < 0)
            {
                // No path at all: add "/{replacement}"
                string prefix = s.Substring(0, suffixStart);
                string suffix = s.Substring(suffixStart);
                return new System.Uri(prefix + "/" + replacement + suffix, UriKind.Absolute);
            }

            if (lastSlash < firstPathSlash)
                lastSlash = firstPathSlash; // replace "empty" last segment at root
        }

        string before = s.Substring(0, lastSlash + 1);
        string after = s.Substring(suffixStart); // includes ?/# or empty

        // Note: we intentionally do NOT add back the trailing slash (it was part of "last segment" region)
        // If you want to preserve it, you can append "/" before 'after' when hadTrailingSlash.
        string result = before + replacement + after;

        return new System.Uri(result, UriKind.Absolute);
    }

    /// <summary>
    /// Returns a new absolute URI with the last path segment removed from the specified URI.
    /// </summary>
    /// <remarks>Query and fragment components, if present, are preserved in the returned URI. If the path
    /// ends with a slash, it is ignored when determining the last segment.</remarks>
    /// <param name="uri">The absolute URI from which to remove the last path segment. Cannot be null.</param>
    /// <returns>A new absolute URI with the last segment of the path removed. If the URI does not contain any path segments, the
    /// original URI is returned.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="uri"/> is null.</exception>
    [Pure]
    public static System.Uri RemoveLastSegment(this System.Uri uri)
    {
        if (uri is null)
            throw new ArgumentNullException(nameof(uri));

        string s = uri.AbsoluteUri;

        int q = s.IndexOf('?');
        int h = s.IndexOf('#');
        int suffixStart;
        if (q < 0)
            suffixStart = h < 0 ? s.Length : h;
        else
            suffixStart = h < 0 ? q : (q < h ? q : h);

        int end = suffixStart;

        // If it ends with '/', ignore it for "last segment" determination
        if (end > 0 && s[end - 1] == '/')
            end--;

        int lastSlash = s.LastIndexOf('/', end - 1);
        if (lastSlash < 0)
            return uri;

        // Keep everything up to the slash (inclusive), plus suffix (?/#)
        string before = s.Substring(0, lastSlash + 1);
        string after = s.Substring(suffixStart);

        string result = before + after;
        return new System.Uri(result, UriKind.Absolute);
    }
}