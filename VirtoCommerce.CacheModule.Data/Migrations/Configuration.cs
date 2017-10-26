namespace VirtoCommerce.CacheModule.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<VirtoCommerce.CacheModule.Data.Repositories.CacheRepositoryImpl>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"Migrations";
        }

        protected override void Seed(VirtoCommerce.CacheModule.Data.Repositories.CacheRepositoryImpl context)
        {
        }
    }
}
