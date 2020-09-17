﻿using challenge.Models;
using System;
using System.Threading.Tasks;

namespace challenge.Repositories
{
    public interface ICompensationRepository
    {
        Compensation GetByEmployeeId(String id);
        Compensation Add(Compensation compensation);
        Task SaveAsync();
    }
}