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

            if (!context.BodyHash.EqualsInConstantTime(context.GenerateBodyHash()))
            {
                throw Error.FailedToValidateBodyHash(context);
            }
        }
    }
}
