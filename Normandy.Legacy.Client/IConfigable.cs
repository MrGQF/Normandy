using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Normandy.Legacy.Client.Interfaces
{
    interface IConfigable
    {
        XElement ExportConfig();
        void ImportConfig(XElement xe);
    }
}
