using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using challenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using challenge.Data;

namespace challenge.Repositories
{
    public class CompensationRepository : ICompensationRepository
    {
        private readonly CompensationContext _compensationContext;
        private readonly ILogger<ICompensationRepository> _logger;

        public CompensationRepository(ILogger<ICompensationRepository> logger, CompensationContext compensationContext)
        {
            _compensationContext = compensationContext;
            _logger = logger;
        }

        public Compensation Add(Compensation compensation)
        {
            compensation.CompensationId = Guid.NewGuid().ToString();

            _compensationContext.Compensations.Add(compensation);
            return compensation;
        }

        public Compensation GetByEmployeeId(string id)
        {
            var a = _compensationContext.Compensations;

            // Had to add the Include statement here to enforce Eager Loading of Employees. Otherwise,
            // Employee attribute can return null
            return _compensationContext.Compensations.Include(x => x.Employee).SingleOrDefault(c => c.Employee.EmployeeId == id);
        }

        public Task SaveAsync()
        {
            return _compensationContext.SaveChangesAsync();
        }

    }
}
