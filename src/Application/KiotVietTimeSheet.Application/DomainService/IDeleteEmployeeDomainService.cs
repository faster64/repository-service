using KiotVietTimeSheet.Application.DomainService.Dto;
using System.Threading.Tasks;

namespace KiotVietTimeSheet.Application.DomainService
{
    public interface IDeleteEmployeeDomainService
    {
        /// <summary>
        /// Xóa nhân viên
        /// </summary>
        /// <param name="deleteEmployeeDomainServiceDto"></param>
        /// <returns></returns>
        Task<bool> DeleteEmployee(DeleteEmployeeDomainServiceDto deleteEmployeeDomainServiceDto);

        /// <summary>
        /// Xóa nhân viên
        /// </summary>
        /// <param name="deleteEmployeesDomainServiceDto"></param>
        /// <returns></returns>
        Task<bool> DeleteEmployees(DeleteEmployeesDomainServiceDto deleteEmployeesDomainServiceDto);

        Task<bool> DeleteEmployeeWithoutPermission(
            DeleteEmployeeDomainServiceDto deleteEmployeeDomainServiceDto);
    }
}
