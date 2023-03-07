using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Runtime.Exception;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Models;
using KiotVietTimeSheet.Domain.AggregatesModels.FingerPrintAggregate.Specifications;
using KiotVietTimeSheet.SharedKernel.Domain;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.CreateUpdateFingerPrint
{
    public class CreateUpdateFingerPrintCommandHandler : BaseCommandHandler,
        IRequestHandler<CreateUpdateFingerPrintCommand, FingerPrintDto>
    {
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;
        private readonly IFingerPrintWriteOnlyRepository _fingerPrintWriteOnlyRepository;
        private readonly IEmployeeReadOnlyRepository _employeeReadOnlyRepository;

        public CreateUpdateFingerPrintCommandHandler(
            IEventDispatcher eventDispatcher,
            IMapper mapper,
            IAuthService authService,
            IFingerPrintWriteOnlyRepository fingerPrintWriteOnlyRepository,
            IEmployeeReadOnlyRepository employeeReadOnlyRepository
        )
            : base(eventDispatcher)
        {
            _mapper = mapper;
            _authService = authService;
            _fingerPrintWriteOnlyRepository = fingerPrintWriteOnlyRepository;
            _employeeReadOnlyRepository = employeeReadOnlyRepository;
        }

        public async Task<FingerPrintDto> Handle(CreateUpdateFingerPrintCommand request, CancellationToken cancellationToken)
        {
            var fingerPrintDto = request.FingerPrint;
            if (fingerPrintDto == null)
            {
                throw new KvTimeSheetException("Lỗi không xác định tham số");
            }

            if (fingerPrintDto.BranchId != _authService.Context.BranchId &&
                ! await _authService.HasAnyPermissionMapWithBranchId(
                    new[]
                    {
                        TimeSheetPermission.FingerPrint_Create,
                        TimeSheetPermission.FingerPrint_Update
                    }, fingerPrintDto.BranchId))
            {
                throw new KvTimeSheetUnAuthorizedException($"Người dùng {_authService.Context.User.UserName} không có quyền thực hiện");
            }

            if ((fingerPrintDto.EmployeeId ?? 0) == 0)
            {
                var deleteFingerPrint = await _fingerPrintWriteOnlyRepository.FindBySpecificationAsync(
                    new FindFingerPrintByBranchIdSpec(fingerPrintDto.BranchId)
                        .And(new FindFingerPrintByFingerCodeSpec(fingerPrintDto.FingerCode))
                );

                if (deleteFingerPrint != null)
                {
                    _fingerPrintWriteOnlyRepository.Delete(deleteFingerPrint);
                    await _fingerPrintWriteOnlyRepository.UnitOfWork.CommitAsync();
                    return _mapper.Map<FingerPrintDto>(deleteFingerPrint);
                }
                else
                {
                    return _mapper.Map<FingerPrintDto>(fingerPrintDto);
                }
            }

            var existEmployeeById = await _employeeReadOnlyRepository.FindByIdAsync(fingerPrintDto.EmployeeId.GetValueOrDefault(), reference: false, includeSoftDelete: true);

            if (existEmployeeById == null)
            {
                NotifyValidationErrors(typeof(FingerPrint), new List<string> { $"Nhân viên không tồn tại" });
                return null;
            }

            if (existEmployeeById.IsDeleted)
            {
                NotifyValidationErrors(typeof(FingerPrint), new List<string> { $"Nhân viên {existEmployeeById.Name.Substring(0, existEmployeeById.Name.Length - 5)} đã bị xóa" });
                return null;
            }

            var fingerPrintModel = new FingerPrint(
                    fingerPrintDto.FingerCode,
                    fingerPrintDto.EmployeeId.GetValueOrDefault(),
                    fingerPrintDto.FingerName,
                    fingerPrintDto.FingerNo
                );

            var existFingerPrintByCode = await _fingerPrintWriteOnlyRepository.FindBySpecificationAsync(
                new FindFingerPrintByBranchIdSpec(fingerPrintDto.BranchId)
                    .And(new FindFingerPrintByFingerCodeSpec(fingerPrintDto.FingerCode))
              );

            var existFingerPrintByEmployee = await _fingerPrintWriteOnlyRepository.FindBySpecificationAsync(
                    new FindFingerPrintByBranchIdSpec(fingerPrintDto.BranchId)
                        .And(new FindFingerPrintByEmployeeIdSpec(fingerPrintDto.EmployeeId.GetValueOrDefault()))
                );

            // TH1. Nếu tài khoản chấm công TK1 đang không gắn với nhân viên nào;và nhân viên NV1 được chọn đang gắn với tài khoản chắm công TK2 khác
            if (existFingerPrintByCode == null && existFingerPrintByEmployee != null)
            {
                existFingerPrintByEmployee.UpdatedFingerCode(fingerPrintModel.FingerCode);
                _fingerPrintWriteOnlyRepository.Update(existFingerPrintByEmployee);
                await _fingerPrintWriteOnlyRepository.UnitOfWork.CommitAsync();
                return _mapper.Map<FingerPrintDto>(existFingerPrintByEmployee);
            }

            // TH2. Nếu tài khoản chấm công TK1 đang không gắn với nhân viên nào; và nhân viên NV1 được chọn đang không gắn với tài khoản chấm công nào
            if (existFingerPrintByCode == null)
            {
                fingerPrintModel.BranchId = fingerPrintDto.BranchId;
                _fingerPrintWriteOnlyRepository.Add(fingerPrintModel);
                await _fingerPrintWriteOnlyRepository.UnitOfWork.CommitAsync();
                return _mapper.Map<FingerPrintDto>(fingerPrintModel);
            }

            // TH3. Nếu tài khoản chấm công TK1 đang được gắn với nhân viên NV1;và người dùng đang đổi từ NV1 sang NV2 , trong khi NV2 cũng đang được gắn với tài khoản chấm công TK2 khác.
            if (existFingerPrintByEmployee != null &&
                existFingerPrintByEmployee.Id != existFingerPrintByCode.Id)
            {
                existFingerPrintByCode.UpdatedEmployee(fingerPrintModel.EmployeeId);
                _fingerPrintWriteOnlyRepository.Update(existFingerPrintByCode);
                _fingerPrintWriteOnlyRepository.Delete(existFingerPrintByEmployee);
                await _fingerPrintWriteOnlyRepository.UnitOfWork.CommitAsync();
                return _mapper.Map<FingerPrintDto>(existFingerPrintByCode);
            }

            // TH4. Nếu tài khoản chấm công TK1 đang được gắn với nhân viên NV1;và người dùng đang đổi từ NV1 sang NV2, trong khi NV2 đang không gắnvới tài khoản chấm công nào khác
            if (existFingerPrintByEmployee == null)
            {
                existFingerPrintByCode.UpdatedEmployee(fingerPrintModel.EmployeeId);
                _fingerPrintWriteOnlyRepository.Update(existFingerPrintByCode);
                await _fingerPrintWriteOnlyRepository.UnitOfWork.CommitAsync();
                return _mapper.Map<FingerPrintDto>(existFingerPrintByCode);
            }

            return _mapper.Map<FingerPrintDto>(fingerPrintModel);
        }
    }
}
