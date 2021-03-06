﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TMog.Entities;

namespace TMog.Services
{
    public interface ISetsService
    {
        Task<IEnumerable<Set>> GetAll();

        Task<Set> GetById(int setId);

        Task<Set> Create(int setId);

        Task MarkSetSlotCompletionStatus(int setId, SlotType slot, bool completed);

        Task Delete(int setId);
    }
}