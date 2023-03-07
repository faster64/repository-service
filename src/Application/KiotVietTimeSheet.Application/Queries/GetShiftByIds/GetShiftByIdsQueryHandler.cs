﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;
using KiotVietTimeSheet.Application.Queries.GetShiftByIds;
using KiotVietTimeSheet.Application.Repositories.ReadRepositories;
using KiotVietTimeSheet.Domain.AggregatesModels.ShiftAggregate.Specifications;
using MediatR;

namespace KiotVietTimeSheet.Application.Queries.GetShiftById
{
    public class GetShiftByIdsQueryHandler : QueryHandlerBase,
        IRequestHandler<GetShiftByIdsQuery, List<ShiftDto>>
    {
        private readonly IShiftReadOnlyRepository _shiftReadOnlyRepository;
        private readonly IMapper _mapper;

        public GetShiftByIdsQueryHandler(
            IAuthService authService,
            IShiftReadOnlyRepository shiftReadOnlyRepository,
            IMapper mapper
        ) : base(authService)
        {
            _shiftReadOnlyRepository = shiftReadOnlyRepository;
            _mapper = mapper;
        }

        public async Task<List<ShiftDto>> Handle(GetShiftByIdsQuery request, CancellationToken cancellationToken)
        {
            var result = await _shiftReadOnlyRepository.GetBySpecificationAsync(new FindShiftByShiftIdsSpec(request.Ids), false, true);
            return _mapper.Map<List<ShiftDto>>(result);
        }
    }
}
