using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Soenneker.Extensions.Uri;

public static class UriExtension
{
    /// <summary>
    /// Returns a new URI in which the last path segment of the specified URI is replaced with the given replacement
    /// string.
    /// </summary>
    /// <remarks>The query and fragment components of the original URI, if present, are preserved in the
    /// returned URI. This method does not modify the original URI instance.</remarks>
    /// <param name="uri">The source URI whose last path segment is to be replaced. Must be an absolute URI.</param>
    /// <param name="replacement">The string to use as the new last path segment. Cannot be null.</param>
    /// <returns>A new absolute URI with the last path segment replaced by the specified replacement string. If the original URI
    /// has no path, the replacement is appended as the first path segment.</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static System.Uri ReplaceLastSegment(this System.Uri uri, string replacement)
    {
        ArgumentNullException.ThrowIfNull(uri);
        ArgumentNullException.ThrowIfNull(replacement);

        // absoluteUri includes query/fragment
        string s = uri.AbsoluteUri;
        ReadOnlySpan<char> span = s.AsSpan();

        int suffixStart = GetSuffixStart(span); // start of ?/# or end
        int end = TrimTrailingSlash(span, suffixStart);

        // Find authority/path boundary
        int firstPathSlash = GetFirstPathSlash(span);

        if (firstPathSlash < 0)
        {
            // No path at all: insert "/{replacement}" before suffix
            int prefixLen = suffixStart;
            int afterLen = span.Length - suffixStart;

            int totalLen = prefixLen + 1 + replacement.Length + afterLen;

            var result = string.Create(totalLen, (s, prefixLen, afterLen, replacement), static (dest, state) =>
            {
                (string src, int pLen, int aLen, string repl) = state;
                src.AsSpan(0, pLen)
                   .CopyTo(dest);
                dest[pLen] = '/';
                repl.AsSpan()
                    .CopyTo(dest.Slice(pLen + 1));
                src.AsSpan(src.Length - aLen, aLen)
                   .CopyTo(dest.Slice(pLen + 1 + repl.Length));
            });

            return new System.Uri(result, UriKind.Absolute);
        }

        // Find last slash (within [0..end))
        int lastSlash = span.Slice(0, end)
                            .LastIndexOf('/');

        // Safety fallback (shouldn’t happen for absolute URIs, but keep it cheap)
        if (lastSlash < 0)
        {
            var result = string.Create(span.Length + 1 + replacement.Length, (s, replacement), static (dest, state) =>
            {
                (string src, string repl) = state;
                src.AsSpan()
                   .CopyTo(dest);
                dest[src.Length] = '/';
                repl.AsSpan()
                    .CopyTo(dest.Slice(src.Length + 1));
            });

            return new System.Uri(result, UriKind.Absolute);
        }

        // If lastSlash lands before the first path slash, treat as root (replace empty segment at "/")
        if (lastSlash < firstPathSlash)
            lastSlash = firstPathSlash;

        int beforeLen = lastSlash + 1; // include slash
        int afterLen2 = span.Length - suffixStart; // includes ?/# or empty

        int totalLen2 = beforeLen + replacement.Length + afterLen2;

        var result2 = string.Create(totalLen2, (s, beforeLen, suffixStart, replacement), static (dest, state) =>
        {
            (string src, int bLen, int sufStart, string repl) = state;

            src.AsSpan(0, bLen)
               .CopyTo(dest);
            repl.AsSpan()
                .CopyTo(dest.Slice(bLen));
            src.AsSpan(sufStart)
               .CopyTo(dest.Slice(bLen + repl.Length));
        });

        return new System.Uri(result2, UriKind.Absolute);
    }

    [Pure, MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static System.Uri RemoveLastSegment(this System.Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);

        string s = uri.AbsoluteUri;
        ReadOnlySpan<char> span = s.AsSpan();

        int suffixStart = GetSuffixStart(span);
        int end = TrimTrailingSlash(span, suffixStart);

        // Find authority/path boundary
        int firstPathSlash = GetFirstPathSlash(span);
        if (firstPathSlash < 0)
            return uri; // no path

        int lastSlash = span.Slice(0, end)
                            .LastIndexOf('/');
        if (lastSlash < 0)
            return uri;

        // If lastSlash lands before the path, there are no path segments to remove
        if (lastSlash < firstPathSlash)
            return uri;

        int beforeLen = lastSlash + 1; // keep trailing slash
        int afterLen = span.Length - suffixStart; // keep ?/#

        // If nothing changes, just return original
        if (beforeLen == suffixStart && afterLen == span.Length - suffixStart)
            return uri;

        int totalLen = beforeLen + afterLen;

        var result = string.Create(totalLen, (s, beforeLen, suffixStart), static (dest, state) =>
        {
            (string src, int bLen, int sufStart) = state;
            src.AsSpan(0, bLen)
               .CopyTo(dest);
            src.AsSpan(sufStart)
               .CopyTo(dest.Slice(bLen));
        });

        return new System.Uri(result, UriKind.Absolute);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetSuffixStart(ReadOnlySpan<char> span)
    {
        int i = span.IndexOfAny('?', '#');
        return i < 0 ? span.Length : i;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int TrimTrailingSlash(ReadOnlySpan<char> span, int suffixStart)
    {
        int end = suffixStart;
        if (end > 0 && span[end - 1] == '/')
            end--;
        return end;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetFirstPathSlash(ReadOnlySpan<char> span)
    {
        // Find "://"
        int schemeSep = span.IndexOf("://".AsSpan());
        if (schemeSep < 0)
            return -1;

        int authorityStart = schemeSep + 3;
        int firstPathSlash = span.Slice(authorityStart)
                                 .IndexOf('/');
        return firstPathSlash < 0 ? -1 : authorityStart + firstPathSlash;
    }
}