using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PaprikaFunctionsApp.Common.Twitter
{
    public class TwitterUser
    {
        public const string GuidPrefix = "b867fea664b1";
        public string Token { get; set; }
        public string TokenSecret { get; set; }
        public long UserId { get; set; }
        public Guid UserGuid
        {
            get { return Guid.Parse(GuidPrefix + UserId.ToString("x").PadLeft(32 - GuidPrefix.Length, '0')); }
            set { UserId = long.Parse(value.ToString("N").Substring(GuidPrefix.Length), NumberStyles.HexNumber); }
        }
        public string ScreenName { get; set; }

        public string UserName
        {
            get { return ScreenName; }
            set { ScreenName = value; }
        }

        public IEnumerable<string> Claims
        {
            get { return Enumerable.Empty<string>(); }
            set { }
        }
    }
}
