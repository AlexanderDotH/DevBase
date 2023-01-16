using System.Text;
using DevBase.Generic;
using DevBase.Web.RequestData.Data;

namespace DevBase.Web.RequestData.Types;

public class FormDataHolder
{
    private GenericList<FormKeypair> _formKeyPairs;

    public FormDataHolder()
    {
        this._formKeyPairs = new GenericList<FormKeypair>();
    }

    public void AddKeyPair(FormKeypair formKeypair)
    {
        this._formKeyPairs.Add(formKeypair);
    }
    
    public void AddKeyPairs(GenericList<FormKeypair> formKeypair)
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