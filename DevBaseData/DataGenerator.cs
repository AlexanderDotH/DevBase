using DevBaseData.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBase.Generic;

namespace DevBaseData
{
    public class DataGenerator
    {
        private DataType[] _dataTypes;

        private List<string> _generatedData;
        private List<IGenerator> _generators;

        public DataGenerator(int dataAmount, int dataSize, DataType[] dataTypes)
        {
            this._dataTypes = dataTypes;

            this._generators = new List<IGenerator>();
            this._generators.Add(new EmailGenerator(dataAmount, dataSize, true));
            this._generators.Add(new PasswordGenerator(dataAmount, dataSize));

            this._generatedData = GenerateData();
        }

        private List<string> GenerateData()
        {
            GenericList<string> generatedData = new GenericList<string>();

            for (int i = 0; i < this._dataTypes.Length; i++)
            {
                switch(this._dataTypes[i])
                {
                    case DataType.Email:
                        generatedData.AddRange(GetGenerator("EmailGenerator").GenerateData());
                        break;
                    case DataType.Password:
                        generatedData.AddRange(GetGenerator("PasswordGenerator").GenerateData());
                        break;
                }
            }

            if (this._dataTypes.Length > 0)
            {
                if (this._dataTypes.Contains(DataType.Email) && 
                    this._dataTypes.Contains(DataType.Password)) 
                {

                }
            }
                 
            return generatedData;
        }

        private IGenerator GetGenerator(string name)
        {
            foreach(IGenerator generator in this._generators) 
            {
                if (generator.Name() == name)
                {
                    return generator;
                }
            }

            return null;
        }

        public List<string> GeneratedData
        {
            get { return this._generatedData; }
        }
    }
}
