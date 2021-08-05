using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevBase.Generic;

namespace DevBaseData.Generators
{
    interface IGenerator
    {
        string Name();
        GenericList<string> GenerateData();
    }
}
