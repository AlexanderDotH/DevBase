using DevBase.Generic;
using Newtonsoft.Json;

namespace DevBase.Api.Helper
{
    public class JsonHelper<T>
    {
        private JsonSerializerSettings _serializerSettings;
        private GenericList<string> _errorList;

        public JsonHelper()
        {
            this._errorList = new GenericList<string>();

            this._serializerSettings = new JsonSerializerSettings();

            this._serializerSettings.NullValueHandling = NullValueHandling.Ignore;

            this._serializerSettings.Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
            {
                this._errorList.Add(args.ErrorContext.Error.Message);
                args.ErrorContext.Handled = true;
            };
        }

        public T Deserialize(string input)
        {
            return JsonConvert.DeserializeObject<T>(input, this._serializerSettings);
        }

        public GenericList<string> ErrorList
        {
            get => _errorList;
            set => _errorList = value;
        }
    }
}
