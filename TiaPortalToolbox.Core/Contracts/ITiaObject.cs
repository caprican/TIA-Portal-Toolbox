using System;
using System.Collections.Generic;
using System.Text;

using Siemens.Engineering.HmiUnified;
using Siemens.Engineering.HmiUnified.UI.Screens;

namespace TiaPortalToolbox.Core.Contracts
{
    interface ITiaObject
    {
        Dictionary<string, List<Dictionary<string, object>>> Export(IEnumerable<HmiScreen> allScreens);
        void Import(HmiSoftware hmiSoftware, List<object> data, IEnumerable<HmiScreen> allScreens);
    }
}
