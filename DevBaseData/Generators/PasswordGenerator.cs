using DevBase.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBase.Generic;

namespace DevBaseData.Generators
{
    class PasswordGenerator : IGenerator
    {
        public string Name() => "PasswordGenerator";

        private int _amount;
        private int _passwordSize;

        private bool _randomSize;

        public PasswordGenerator(int amount, int passwordSize, bool randomSize)
        {
            this._amount = amount;
            this._passwordSize = passwordSize;

            this._randomSize = randomSize;
        }

        public PasswordGenerator(int amount, int passwordSize) : this(amount, passwordSize, false) {}

        public GenericList<string> GenerateData()
        {
            GenericList<string> generatedData = new GenericList<string>();

            Random random = new Random();

            for (int i = 0; i < this._amount; i++)
            {
                if (this._randomSize)
                {
                    generatedData.Add(StringUtils.RandomString(random.Next(1, this._passwordSize)));
                }
                else
                {
                    generatedData.Add(StringUtils.RandomString(this._passwordSize));
                }
            }

            return generatedData;
        }

    }
}
