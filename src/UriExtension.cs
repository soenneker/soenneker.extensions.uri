using System.Diagnostics.Contracts;
using Soenneker.Utils.RegexCollection;

namespace Soenneker.Extensions.Uri;

/// <summary>
/// A collection of useful Uri extension methods
/// </summary>
public static class UriExtension
{
    [Pure]
    public static System.Uri ReplaceLastSegment(this System.Uri uri, string replacement)
    {
        string regexResult = RegexCollection.UriLastSegment().Replace(uri.AbsoluteUri, $"$1/{replacement}$3");

        var rtnUri = new System.Uri(regexResult);
        return rtnUri;
    }

    [Pure]
    public static System.Uri RemoveLastSegment(this System.Uri uri)
    {
        string regexResult = RegexCollection.UriLastSegment().Replace(uri.AbsoluteUri, "$1$3");

        var rtnUri = new System.Uri(regexResult);
        return rtnUri;
    }
}