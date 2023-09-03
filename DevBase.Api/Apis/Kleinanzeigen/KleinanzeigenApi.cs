using System.Text;

namespace DevBase.Api.Apis.Kleinanzeigen;

public class KleinanzeigenApi
{
    private readonly string _baseUrl;

    private readonly string _username;
    private readonly string _password;

    public KleinanzeigenApi()
    {
        this._baseUrl = "https://api.ebay-kleinanzeigen.de/api";

        this._username = "android";
        this._password = "TaR60pEttY";
    }

    public async Task Search(string term)
    {
        
    }
    
    private string BuildAuthHeader() =>  Convert.ToBase64String(
        Encoding.UTF8.GetBytes(string.Format("{0}:{1}", this._username, this._password)));
}