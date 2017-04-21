using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtoCommerce.CacheModule.Web.Model
{
    public class CacheHandleStat
    {
        /// <summary>
        /// Handle name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The number of hits.
        /// </summary>
        public long Hits { get; set; }
        /// <summary>
        /// The number of misses.
        /// </summary>
        public long Misses { get; set; }
        //Gets the number of items the cache  currently maintains.
        public long ItemsCount { get; set; }
    }
}