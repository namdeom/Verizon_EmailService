﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace Services.EmailServiceProvider.Interface
{
    [ExcludeFromCodeCoverage]
    public class FakeBadEmailDomainsCheckProvider : IBadEmailDomainsCheckProvider
    {
        public bool IsBadDomain(string eMailAddress)
        {
            return false;
        }
    }
}
