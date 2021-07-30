using DevBase.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevBaseData.Generators
{
    class PasswordGenerator : IGenerator
    {
        public string Name() => "PasswordGenerator";

        private int _amount;
        private int _passwordSize;

        public PasswordGenerator(int amount, int passwordSize)
        {
            this._amount = amount;
            this._passwordSize = passwordSize;
        }

        public List<string> GenerateData()
        {
            List<string> generatedData = new List<string>();

            Random random = new Random();

            for (int i = 0; i < this._amount; i++)
            {
                generatedData.Add(StringUtils.RandomString(this._passwordSize));
            }

            return generatedData;
        }

    }
}
