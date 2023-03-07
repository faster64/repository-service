using System.Threading;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.RemoveEmployeePictureId
{
    public class RemoveEmployeePictureIdCommandHandler : BaseCommandHandler,
        IRequestHandler<RemoveEmployeePictureIdCommand, Unit>
    {
        private readonly IEmployeeProfilePictureWriteOnlyRepository _employeeProfilePictureWriteOnlyRepository;
        private readonly IEmployeeWriteOnlyRepository _employeeWriteOnlyRepository;
        public RemoveEmployeePictureIdCommandHandler(
            IEventDispatcher eventDispatcher,
            IEmployeeProfilePictureWriteOnlyRepository employeeProfilePictureWriteOnlyRepository,
            IEmployeeWriteOnlyRepository employeeWriteOnlyRepository)
            : base(eventDispatcher)
        {
            _employeeProfilePictureWriteOnlyRepository = employeeProfilePictureWriteOnlyRepository;
            _employeeWriteOnlyRepository = employeeWriteOnlyRepository;
        }

        public async Task<Unit> Handle(RemoveEmployeePictureIdCommand request, CancellationToken cancellationToken)
        {
            var employeeProfilePicture = await _employeeProfilePictureWriteOnlyRepository.FindByIdAsync(request.Id);
            if (employeeProfilePicture != null)
            {
                _employeeProfilePictureWriteOnlyRepository.Delete(employeeProfilePicture);
                var employee = await _employeeWriteOnlyRepository.FindByIdAsync(employeeProfilePicture.EmployeeId);
                employee.UpdateProfilePicture();
                _employeeWriteOnlyRepository.Update(employee);

                await _employeeProfilePictureWriteOnlyRepository.UnitOfWork.CommitAsync();
            }
            return Unit.Value;
        }
    }
}
