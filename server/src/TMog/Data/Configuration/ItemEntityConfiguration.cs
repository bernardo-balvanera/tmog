﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using TMog.Entities;

namespace TMog.Data.Configuration
{
    internal class ItemEntityConfiguration : EntityTypeConfiguration<Item>
    {
        public ItemEntityConfiguration()
        {
            HasKey(i => i.ItemId).Property(i => i.ItemId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
            HasMany(i => i.Sources).WithMany().Map(config => config.MapLeftKey("ItemId").MapRightKey("SourceId").ToTable("ItemSources"));
        }
    }
}
