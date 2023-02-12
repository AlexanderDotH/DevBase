using DevBase.Enums;
using DevBase.Generics;
using DevBase.Web.RequestData.Data;

namespace DevBase.Web.RequestData.Types;

public class AuthMethodHolder
{
    private Auth _authData;

    private ATupleList<EnumAuthType, string> _authDictionary;

    public AuthMethodHolder(Auth auth)
    {
        this._authData = auth;

        this._authDictionary = new ATupleList<EnumAuthType, string>();
        this._authDictionary.Add(EnumAuthType.BASIC, "Basic");
        this._authDictionary.Add(EnumAuthType.OAUTH2, "Bearer");
    }

    public string GetAuthData()
    {
        return string.Format("{0} {1}", this._authDictionary.FindEntry(this._authData.AuthType), this._authData.Token);
    }
}