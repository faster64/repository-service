using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.DomainService.Dto;
using KiotVietTimeSheet.Application.EventBus.Events.EmployeeEvents;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Enums;
using KiotVietTimeSheet.Domain.AggregatesModels.ClockingAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Events;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.EmployeeAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Specifications;
using KiotVietTimeSheet.Domain.AggregatesModels.TimeSheetAggregate.Specifications;
using System.Linq;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.DomainService
{
    public class DeleteEmployeeDomainService : IDeleteEmployeeDomainService
    {
        private readonly IEmployeeWriteOnlyRepository _employeeWriteOnlyRepository;
        private readonly IClockingWriteOnlyRepository _clockingWriteOnlyRepository;
        private readonly ITimeSheetWriteOnlyRepository _timeSheetWriteOnlyRepository;
        private readonly IFingerPrintWriteOnlyRepository _fingerPrintWriteOnlyRepository;
        private readonly ITimeSheetIntegrationEventService _timeSheetIntegrationEventService;

        public DeleteEmployeeDomainService(
            IEmployeeWriteOnlyRepository employeeWriteOnlyRepository,
            IClockingWriteOnlyRepository clockingWriteOnlyRepository,
            ITimeSheetWriteOnlyRepository timeSheetWriteOnlyRepository,
            IFingerPrintWriteOnlyRepository fingerPrintWriteOnlyRepository,
            ITimeSheetIntegrationEventService timeSheetIntegrationEventService
            )
        {
            _employeeWriteOnlyRepository = employeeWriteOnlyRepository;
            _clockingWriteOnlyRepository = clockingWriteOnlyRepository;
            _timeSheetWriteOnlyRepository = timeSheetWriteOnlyRepository;
            _fingerPrintWriteOnlyRepository = fingerPrintWriteOnlyRepository;
            _timeSheetIntegrationEventService = timeSheetIntegrationEventService;
        }

        public async Task<bool> DeleteEmployee(DeleteEmployeeDomainServiceDto deleteEmployeeDomainServiceDto)
        {
            var employee = await _employeeWriteOnlyRepository.FindByIdAsync(deleteEmployeeDomainServiceDto.Id);
            if (employee == null)
            {
                return false;
            }

            await CancelTimesheet(employee);
            await DeleteClockings(employee);

            // Audit log
            await _timeSheetIntegrationEventService.AddEventAsync(new DeletedEmployeeIntegrationEvent(new DeletedEmployeeEvent(employee)));

            var fingerPrints = await _fingerPrintWriteOnlyRepository.GetBySpecificationAsync(new FindFingerPrintByEmployeeIdSpec(employee.Id));

            _fingerPrintWriteOnlyRepository.BatchDelete(fingerPrints);

            employee.Delete();
            _employeeWriteOnlyRepository.Delete(employee);

            return true;
        }

        public async Task<bool> DeleteEmployeeWithoutPermission(
            DeleteEmployeeDomainServiceDto deleteEmployeeDomainServiceDto)
        {
            var employee = await _employeeWriteOnlyRepository.FindByIdWithoutPermission(deleteEmployeeDomainServiceDto.Id);
            if (employee == null)
            {
                return false;
            }

            var timeSheets =
                await _timeSheetWriteOnlyRepository.FindByIdWithoutPermission(deleteEmployeeDomainServiceDto.Id);
            foreach (var timeSheet in timeSheets)
            {
                timeSheet.Cancel();
            }
            _timeSheetWriteOnlyRepository.BatchUpdate(timeSheets);
            var clockings =
                await _clockingWriteOnlyRepository.FindByIdAndStatusWithoutPermission(employee.Id,
                    (byte)ClockingStatuses.Created);
            _clockingWriteOnlyRepository.BatchDelete(clockings);

            var fingerPrints = await _fingerPrintWriteOnlyRepository.FindByIdWithoutPermission(employee.Id);

            _fingerPrintWriteOnlyRepository.BatchDelete(fingerPrints);

            employee.Delete();
            _employeeWriteOnlyRepository.Delete(employee);

            return true;
        }

        public async Task<bool> DeleteEmployees(DeleteEmployeesDomainServiceDto deleteEmployeesDomainServiceDto)
        {
            var employees = await _employeeWriteOnlyRepository.GetBySpecificationAsync(new GetByLongIdsSpec(deleteEmployeesDomainServiceDto.Ids));

            if (employees == null || !employees.Any()) return false;
            foreach (var emp in employees)
            {
                await CancelTimesheet(emp);
                await DeleteClockings(emp);
                emp.Delete();
            }

            // Audit log
            await _timeSheetIntegrationEventService.AddEventAsync(new DeletedMultipleEmployeeIntegrationEvent(employees));

            var fingerPrints = await _fingerPrintWriteOnlyRepository.GetBySpecificationAsync(new FindFingerPrintByEmployeeIdsSpec(employees.Select(x => x.Id).ToList()));

            _fingerPrintWriteOnlyRepository.BatchDelete(fingerPrints);
            _employeeWriteOnlyRepository.BatchDelete(employees);

            return true;
        }

        private async Task CancelTimesheet(Employee employee)
        {
            var timeSheets = await _timeSheetWriteOnlyRepository.GetBySpecificationAsync(new FindTimeSheetByEmployeeIdSpec(employee.Id));
            foreach (var timeSheet in timeSheets)
            {
                timeSheet.Cancel();
            }
            // Hủy toàn bộ lịch làm việc của nhân viên
            _timeSheetWriteOnlyRepository.BatchUpdate(timeSheets);
        }

        private async Task DeleteClockings(Employee employee)
        {
            var clockings = await _clockingWriteOnlyRepository.GetBySpecificationAsync(new FindClockingByEmployeeIdSpec(employee.Id).And(new FindClockingByStatusSpec((byte)ClockingStatuses.Created)));
            // Xóa các chi tiết làm việc trong trạng thái đã đặt lịch của nhân viên này
            _clockingWriteOnlyRepository.BatchDelete(clockings);
        }
    }
}
