using Soenneker.Tests.Unit;
using System.Threading;

namespace Soenneker.Extensions.Uri.Tests;

public class UriExtensionTests : UnitTest
{
    [Test]
    public async System.Threading.Tasks.ValueTask ReplaceLastSegment_EscapesSegmentData(CancellationToken cancellationToken)
    {
        var uri = new System.Uri("https://example.com/api/old?download=1#result");

        System.Uri result = uri.ReplaceLastSegment("folder/report?.pdf");

        await Assert.That(result.AbsoluteUri).IsEqualTo("https://example.com/api/folder%2Freport%3F.pdf?download=1#result");
    }

    [Test]
    public async System.Threading.Tasks.ValueTask ReplaceLastSegment_RejectsDotDotNavigation(CancellationToken cancellationToken)
    {
        var uri = new System.Uri("https://example.com/api/old");
        var threw = false;

        try
        {
            uri.ReplaceLastSegment("..");
        }
        catch (System.UriFormatException)
        {
            threw = true;
        }

        await Assert.That(threw).IsTrue();
    }
}
