using System;
using System.Collections.Generic;
using System.Text;

namespace UrlShortener.Logic
{
    public class Enum
    {
        public enum UrlErrorType
        {
            Other = 1,
            MissingDNS = 2,
            MissingProtocol = 3
        }

    }
}
