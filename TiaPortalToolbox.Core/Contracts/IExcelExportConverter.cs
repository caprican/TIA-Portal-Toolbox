using System;
using System.Collections.Generic;
using System.Text;

namespace TiaPortalToolbox.Core.Contracts
{
    interface IExcelExportConverter
    {
        void Write(Dictionary<string, List<Dictionary<string, object>>> data, string path);
        List<object> Read(string path);
    }
}
