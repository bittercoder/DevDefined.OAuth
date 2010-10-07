using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevDefined.OAuth.Framework;
using DevDefined.OAuth.Utility;

namespace DevDefined.OAuth.Provider.Inspectors
{
    public class BodyHashValidationInspector: IContextInspector
    {
        public void InspectContext(ProviderPhase phase, IOAuthContext context)
        {
            if (context.SignatureMethod == SignatureMethod.PlainText ||
                String.IsNullOrEmpty(context.BodyHash)) return;

            var computedBodyHash = UriUtility.UrlEncode(Convert.ToBase64String(context.RawContent ?? new byte[0]));

            if (!context.BodyHash.EqualsInConstantTime(computedBodyHash))
            {
                throw Error.FailedToValidateBodyHash(context);
            }
               
        }
    }
}
