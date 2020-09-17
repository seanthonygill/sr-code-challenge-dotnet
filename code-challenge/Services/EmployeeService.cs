using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using challenge.Models;
using Microsoft.Extensions.Logging;
using challenge.Repositories;

namespace challenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }

        public ReportingStructure GetDirectReportsById(string id)
        {

            // Use the already existing method to get the Employee
            // by their ID
            Employee foundEmployee = GetById(id);

            if(foundEmployee != null)
            {

                var reportStruct = new ReportingStructure()
                {
                    Employee = foundEmployee,
                    NumberOfReports = calcNumberOfReports(foundEmployee)
                };

                return reportStruct;

            }
            else
            {
                return null;
            }

        }

        /*
         * 
         *  calcNumberOfReports
         *  
         * This function serves as a helper function to recursively calcuate
         * the number of reports of an employee. The function recursively traverses
         * each sub employee until reaching the leaf of the tree, and in the end
         * sums up the number of employees.
         * 
         * @param aEmployee - the Employee to traverse for report count
         * 
         * 
         */
        private int calcNumberOfReports(Employee aEmployee)
        {
            
            // initialize the number of reports to 0
            int sumOfReports = 0;

            // recursion base case - we want to stop recursing if we have reached an
            // employee who has no employees under them
            if (aEmployee.DirectReports == null || aEmployee.DirectReports.Count == 0)
            {
                return 0;
            }
            else
            {
                // use recursion by calling the function for each sub-employee
                foreach (Employee e in aEmployee.DirectReports)
                {
                    // Need to use the GetById function here in order to properly
                    // get the entire Employee object with their direct reports.
                    // Otherwise, entity was returning null for direct reports past
                    // the first level
                    Employee tempEmployee = GetById(e.EmployeeId);

                    // Add 1 to maintain proper count
                    sumOfReports += calcNumberOfReports(tempEmployee) + 1;
                }

                return sumOfReports;

            }

        }

    }
}
