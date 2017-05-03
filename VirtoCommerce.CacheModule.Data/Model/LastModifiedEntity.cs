using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.CacheModule.Data.Model
{
    public class LastModifiedEntity : Entity
    {
        [StringLength(256)]
        [Index(IsUnique = true)]
        public string Scope { get; set; }

        [ConcurrencyCheck]
        public DateTime LastModifiedDate { get; set; }
    }
}
