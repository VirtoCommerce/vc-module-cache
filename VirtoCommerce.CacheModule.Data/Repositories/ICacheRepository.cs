using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.CacheModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CacheModule.Data.Repositories
{
    public interface ICacheRepository : IRepository
    {
        LastModifiedEntity GetLastModifiedForScope(string scope);
    }
}
