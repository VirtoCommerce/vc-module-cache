using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtoCommerce.CacheModule.Web.Decorators
{
    internal interface ICachedServiceDecorator
    {     
       void ClearCache();
    }
}