using System.Text;
using DevBase.Generics;
using DevBase.Web.RequestData.Data;

namespace DevBase.Web.RequestData.Types;

public class FormDataHolder
{
    private AList<FormKeypair> _formKeyPairs;

    public FormDataHolder()
    {
        this._formKeyPairs = new AList<FormKeypair>();
    }

    public void AddKeyPair(FormKeypair formKeypair)
    {
        this._formKeyPairs.Add(formKeypair);
    }
    
    public void AddKeyPairs(AList<FormKeypair> formKeypair)
    {
        this._formKeyPairs.AddRange(formKeypair);
    }
    
    public string GetKeyPairs()
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < this._formKeyPairs.Length; i++)
        {
            FormKeypair keypair = this._formKeyPairs.Get(i);

            if (i == 0)
            {
                sb.Append(keypair.Key + "=" + keypair.Value);
            }
            else
            {
                sb.Append("&" + keypair.Key + "=" + keypair.Value);
            }
        }

        return sb.ToString();
    }
}