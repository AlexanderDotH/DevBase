using DevBase.Format.Structure;
using DevBase.Generics;
using Dumpify;
using Org.BouncyCastle.Asn1.X509;

namespace DevBase.Test.DevBaseApi.BeatifulLyrics;

public class BeautifulLyricsTests
{
    [Test]
    public async Task GetRawLyricsTest()
    {
        Api.Apis.BeautifulLyrics.BeautifulLyrics beautifulLyrics = new Api.Apis.BeautifulLyrics.BeautifulLyrics();

        var rawLyrics = await beautifulLyrics.GetRawLyrics("QZFZ32013014");

        rawLyrics.DumpConsole();
        
        Assert.NotNull(rawLyrics);
    }
    
    [Test]
    public async Task GetTimeStampedLyricsTest()
    {
        Api.Apis.BeautifulLyrics.BeautifulLyrics beautifulLyrics = new Api.Apis.BeautifulLyrics.BeautifulLyrics();

        var timeStampedLyrics = await beautifulLyrics.GetLyrics("QZFZ32013014");

        if (timeStampedLyrics is AList<TimeStampedLyric> stampedLyrics)
            stampedLyrics.DumpConsole();
        
        Assert.NotNull(timeStampedLyrics);
    }
    
    [Test]
    public async Task GetRichTimeStampedLyricsTest()
    {
        Api.Apis.BeautifulLyrics.BeautifulLyrics beautifulLyrics = new Api.Apis.BeautifulLyrics.BeautifulLyrics();

        var richTimeStampedLyrics = await beautifulLyrics.GetLyrics("GBARL9300135");

        if (richTimeStampedLyrics is AList<RichTimeStampedLyric> richTimeStamped)
            richTimeStamped.DumpConsole();
        
        Assert.NotNull(richTimeStampedLyrics);
    }
}