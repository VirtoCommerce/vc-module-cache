using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CacheModule.Data.Extensions
{
    public static class SerializationExtension
    {
        public static string ToJson<T>(this T source) where T : class, new()
        {
            var retVal = string.Empty;
            using (var memStream = new MemoryStream())
            {
                source.SerializeJson(memStream);
                memStream.Seek(0, SeekOrigin.Begin);
                retVal = memStream.ReadToString();
            }
            return retVal;
        }

     
    }
}