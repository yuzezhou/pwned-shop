using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pwned_shop.Utils
{
    public class ShortGuid
    {
        // shorten Guid and make it url friendly
        public static string Shorten(Guid guid)
        {
            var shortGuid = Convert.ToBase64String(guid.ToByteArray());

            // replace url unfriendly characters with friendly ones
            shortGuid = shortGuid.Replace('+', '-').Replace('/', '_');

            return shortGuid.Substring(0, shortGuid.Length - 2);
        }
    }
}
