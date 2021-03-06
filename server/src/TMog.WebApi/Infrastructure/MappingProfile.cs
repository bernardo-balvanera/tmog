﻿using System;
using System.Collections.Generic;
using System.Linq;
using TMog.Common;
using TMog.Common;
using TMog.Entities;
using TMog.WebApi.Models;

namespace TMog.WebApi.Infrastructure
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<Set, TmogSet>()
                .ForMember(target => target.Id, opt => opt.MapFrom(source => source.SetId))
                .ForMember(target => target.Slots, opt => opt.Ignore())
                .ForMember(target => target.TotalSlots, opt => opt.ResolveUsing(source =>
                {
                    var slots = new SlotManager(source.Slots);
                    return slots.ActiveSlotCount;
                }))
                .ForMember(target => target.CompletedSlots, opt => opt.ResolveUsing(source =>
                {
                    var slots = new SlotManager(source.Slots);
                    return slots.CompletedSlotCount;
                }));

            CreateMap<Entities.Item, Models.Item>()
                .ForMember(target => target.Id, opt => opt.MapFrom(source => source.ItemId));

            CreateMap<Entities.Source, Models.Source>()
                .ForMember(target => target.Id, opt => opt.MapFrom(source => source.SourceId))
                .ForMember(target => target.DropLevelName, opt => opt.ResolveUsing(source => source.DropLevel?.GetDisplayValue()))
                .ForMember(target => target.SubType, opt => opt.ResolveUsing(source => source.SubType?.GetDisplayValue()));

            CreateMap<Entities.Zone, Models.Zone>()
                .ForMember(target => target.Id, opt => opt.MapFrom(source => source.ZoneId))
                .ForMember(target => target.ParentZoneName, opt => opt.ResolveUsing(source => source.Parent?.Name))
                .ForMember(target => target.ParentId, opt => opt.ResolveUsing(source => source.Parent?.ZoneId));

            CreateMap<Entities.Faction, Models.Faction>()
                .ForMember(target => target.Id, opt => opt.MapFrom(source => source.FactionId))
                .ReverseMap();

            CreateMap<Models.RawWorldQuest, Entities.WorldQuest>()
                .ForMember(target => target.QuestId, opt => opt.MapFrom(source => source.Id))
                .ForMember(target => target.Side, opt => opt.ResolveUsing(source => WowSide.Both))
                .ForMember(target => target.Category, opt => opt.ResolveUsing(source => (WorldQuestCategory)source.Type))
                .ForMember(target => target.Factions, opt => opt.ResolveUsing(source =>
                {
                    return source.Factions?.Select(faction => new Entities.Faction { FactionId = faction }).ToList();
                }))
                .ForMember(target => target.Zone, opt => opt.ResolveUsing(source =>
                {
                    if (source.Zones == null || source.Zones.Count() == 0)
                    {
                        return null;
                    }

                    return new Entities.Zone { ZoneId = source.Zones.FirstOrDefault() };
                }))
                .ForMember(target => target.Instances, opt => opt.ResolveUsing(source =>
                {
                    var epoch      = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    var expiration = epoch.AddMilliseconds(source.Ending.Value).ToLocalTime();

                    return new List<WorldQuestInstance>
                    {
                        new WorldQuestInstance
                        {
                            ExpiresOn = expiration,
                            QuestId   = source.Id.Value,
                            Rewards   = source.Rewards?.Select(reward => new Entities.Item { ItemId = reward.Id.Value }).ToList()
                        }
                    };
                }));

            CreateMap<Entities.WorldQuest, Models.WorldQuest>()
                .ForMember(target => target.WorldQuestId, opt => opt.MapFrom(source => source.QuestId))
                .ForMember(target => target.ExpiresOn, opt => opt.MapFrom(source => source.Instances.Single().ExpiresOn))
                .ForMember(target => target.Rewards, opt => opt.ResolveUsing(source =>
                    source.Instances.Single().Rewards.Select(item => new WorldQuestReward
                    {
                        Id = item.ItemId,
                        Name = item.Name,
                        Quality = (int)item.Quality
                    })
                ));
                

            CreateMap<Entities.Views.ZoneItem, ZoneItem>()
                .ForMember(target => target.Slot, opt => opt.ResolveUsing(source => source.Slot.GetDisplayValue()))
                .ForMember(target => target.SourceType, opt => opt.ResolveUsing(source => source.SourceType?.GetDisplayValue()))
                .ForMember(target => target.SourceSubType, opt => opt.ResolveUsing(source => source.SourceSubType?.GetDisplayValue()));

            CreateMap<Entities.Views.ItemsByZone, ZoneItems>()
                .ForMember(target => target.Id, opt => opt.MapFrom(source => source.ZoneId))
                .ForMember(target => target.Name, opt => opt.MapFrom(source => source.ZoneName))
                .ForMember(target => target.Difficulty, opt => opt.MapFrom(source => source.ZoneDifficulty));

            CreateMap<Entities.Views.ZonesByRegion, Models.Region>()
                .ForMember(target => target.Id, opt => opt.MapFrom(source => source.RegionId))
                .ForMember(target => target.Name, opt => opt.MapFrom(source => source.RegionName));

            
        }
    }
}
