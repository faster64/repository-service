using System;
using System.Linq;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.Utilities;
using Microsoft.EntityFrameworkCore;

namespace KiotVietTimeSheet.Infrastructure.DbRetail
{
    public class RetailDbService : IRetailDbService
    {
        private readonly DbRetailContext _dbRetailContext;
        private readonly IAuthService _authService;

        public RetailDbService(DbRetailContext dbRetailContext, IAuthService authService)
        {
            _dbRetailContext = dbRetailContext;
            _authService = authService;
        }

        public Task<bool> HasDuplicateTaskExportBySessionId(string sessionId, string revision, string type)
        {
            var rev = !string.IsNullOrEmpty(revision) ? Convert.FromBase64String(revision) : null;
            var query = _dbRetailContext.Set<ImportExportFile>() as IQueryable<ImportExportFile>;
            query = query.Where(x => x.CreatedBy == _authService.Context.User.Id && x.KvSessionId == sessionId && x.Revision.Compare(rev) >= 0);
            query = query.Where(x => x.IsImport != true && x.EventType == type && x.Status == (byte)ImportExportStatus.Processing);
            var result = query.AnyAsync();

            return result;
        }

        public Task<bool> HasDuplicateTaskImportBySessionId(string sessionId, string type)
        {
            var query = _dbRetailContext.Set<ImportExportFile>() as IQueryable<ImportExportFile>;
            query = query.Where(x => x.CreatedBy == _authService.Context.User.Id && x.KvSessionId == sessionId);
            query = query.Where(x => x.IsImport == true && x.EventType == type && x.Status == (byte)ImportExportStatus.Processing);
            var result = query.AnyAsync();

            return result;
        }

        public async Task<ImportExportFile> AddImportExportFile(ImportExportFile entity)
        {
            SetCreateTrail(entity);
            await _dbRetailContext.ImportExportFile.AddAsync(entity);
            await _dbRetailContext.SaveChangesAsync();

            return entity;
        }

        private void SetCreateTrail<TEntity>(TEntity entity)
        {
            if (entity is IRetailerId tenantEntity)
                tenantEntity.RetailerId = _authService.Context.TenantId;

            if (entity is IBranchId branchEntity && branchEntity.BranchId == 0)
                branchEntity.BranchId = _authService.Context.BranchId;

            if (entity is ICreatedBy createdByEntity)
                createdByEntity.CreatedBy = _authService.Context.User.Id;

            if (entity is ICreatedDate createdDateEntity)
                createdDateEntity.CreatedDate = DateTime.Now;
        }
    }
}
