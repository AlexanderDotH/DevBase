using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBase.Utilities;

namespace DevBaseData.Generators
{
    public class EmailGenerator : IGenerator
    {
        private int _amount;
        private int _emailSize;
        private bool _randomProviders;

        private string[] _mailProviders;

        public string Name() => "EmailGenerator";

        public EmailGenerator(int amount, int emailSize, bool randomProviders)
        {
            this._amount = amount;
            this._emailSize = emailSize;
            this._randomProviders = randomProviders;

            this._mailProviders = new string[] { "gmail.com", "yahoo.com", "gmx.com" };
        }

        public List<string> GenerateData()
        {
            List<string> generatedData = new List<string>();

            Random random = new Random();

            for (int i = 0; i < this._amount; i++)
            {
                StringBuilder emailAdress = new StringBuilder();

                emailAdress.Append(StringUtils.RandomString(this._emailSize));
                emailAdress.Append("@");

                if (this._randomProviders)
                {
                    emailAdress.Append(this._mailProviders[random.Next(0, this._mailProviders.Length)]);
                } 
                else
                {
                    emailAdress.Append(this._mailProviders[0]);
                }

                generatedData.Add(emailAdress.ToString());
            }

            return generatedData;
        }
    }
}
