using System.Text;

namespace DevBase.Requests.Preparation.Header.UserAgent.Bogus;

public class BogusUserAgentHeaderBuilder
{
    private StringBuilder _preGeneratedUserAgent;

    private bool _randomProduct;
    private bool _randomVersion;
    private bool _randomPlattform;
    private bool _randomUserAgentType;
}