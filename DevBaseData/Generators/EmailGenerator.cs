using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBase.Generic;
using DevBase.Utilities;

namespace DevBaseData.Generators
{
    public class EmailGenerator : IGenerator
    {
        private int _amount;
        private int _emailSize;
        private bool _randomProviders;

        private string[] _mailProviders;

        private bool _randomSize;

        public string Name() => "EmailGenerator";

        public EmailGenerator(int amount, int emailSize, bool randomProviders, bool randomSize)
        {
            this._amount = amount;
            this._emailSize = emailSize;
            this._randomProviders = randomProviders;

            this._mailProviders = new string[] { "gmail.com", "yahoo.com", "gmx.com" };

            this._randomSize = randomSize;
        }

        public EmailGenerator(int amount, int emailSize, bool randomProviders) : this(amount, emailSize, randomProviders, false) { }

        public GenericList<string> GenerateData()
        {
            GenericList<string> generatedData = new GenericList<string>();

            Random random = new Random();

            for (int i = 0; i < this._amount; i++)
            {
                StringBuilder emailAdress = new StringBuilder();

                if (this._randomSize)
                {
                    emailAdress.Append(StringUtils.RandomString(random.Next(1, _emailSize)));
                }
                else
                {
                    emailAdress.Append(StringUtils.RandomString(this._emailSize));
                }

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
