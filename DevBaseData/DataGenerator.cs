using DevBaseData.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBase.Generic;
using DevBase.Utilities;

namespace DevBaseData
{
    public class DataGenerator
    {
        private DataType[] _dataTypes;

        private List<string> _generatedData;
        private List<IGenerator> _generators;

        private bool _randomSize;

        public DataGenerator(int dataAmount, int dataSize, DataType[] dataTypes, bool randomSize)
        {
            this._dataTypes = dataTypes;

            this._generators = new List<IGenerator>();
            this._generators.Add(new EmailGenerator(dataAmount, dataSize, true, randomSize));
            this._generators.Add(new PasswordGenerator(dataAmount, dataSize, randomSize));

            this._randomSize = randomSize;

            this._generatedData = GenerateData();
        }

        public DataGenerator(int dataAmount, int dataSize, DataType[] dataTypes) : this(dataAmount, dataSize, dataTypes, false) { }

        private List<string> GenerateData()
        {
            GenericList<string> generatedData = new GenericList<string>();

            GenericList<string> emailData = null;
            GenericList<string> passwordData = null;

            for (int i = 0; i < this._dataTypes.Length; i++)
            {
                switch(this._dataTypes[i])
                {
                    case DataType.Email:
                        emailData = GetGenerator("EmailGenerator").GenerateData();
                        generatedData.AddRange(emailData.GetAsList());
                        break;
                    case DataType.Password:
                        passwordData = GetGenerator("PasswordGenerator").GenerateData();
                        generatedData.AddRange(passwordData.GetAsList());
                        break;
                }
            }

            if (this._dataTypes != null)
            {
                if (this._dataTypes.Contains(DataType.Email) && 
                    this._dataTypes.Contains(DataType.Password)) 
                {
                    if (emailData != null && 
                        passwordData != null)
                    {
                        generatedData = CollectionUtils.MergeList(emailData.GetAsList(), passwordData.GetAsList(), ":");
                    }
                }
            }
                 
            return generatedData.GetAsList();
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
