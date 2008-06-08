using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevDefined.OAuth.Core;

namespace DevDefined.OAuth.Provider
{
    public interface IContextInspector
    {
        void InspectContext(OAuthContext context);
    }
}
