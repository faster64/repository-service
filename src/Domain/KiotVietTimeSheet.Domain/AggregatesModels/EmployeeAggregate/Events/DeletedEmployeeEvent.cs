﻿using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Domain;

namespace KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Events
{
    public class DeletedEmployeeEvent : DomainEvent
    {
        public Employee Employee { get; set; }
        public DeletedEmployeeEvent(Employee employee)
        {
            Employee = employee;
        }
    }
}
