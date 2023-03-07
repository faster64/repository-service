using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.UnitOfWork;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.SharedKernel.Interfaces;
using KiotVietTimeSheet.SharedKernel.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;
using KiotVietTimeSheet.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Repositories.WriteRepositories;
using KiotVietTimeSheet.Application.Runtime.Exception;
using KiotVietTimeSheet.Domain.AggregatesModels.BranchSettingAggregate.Models;
using Message = KiotVietTimeSheet.Resources.Message;
using Microsoft.Extensions.Logging;

namespace KiotVietTimeSheet.Infrastructure.Persistence.Ef
{
    public class EfBaseWriteOnlyRepository<TEntity> : IBaseWriteOnlyRepository<TEntity>, IDisposable where TEntity : BaseEntity
    {
        #region Properties
        protected IAuthService AuthService;
        private readonly ILogger<EfBaseWriteOnlyRepository<TEntity>> _logger;
        private const int CodePad = 6;
        protected readonly EfDbContext Db;
        public IUnitOfWork UnitOfWork => Db;
        #endregion


        public EfBaseWriteOnlyRepository(EfDbContext db, IAuthService authService, ILogger<EfBaseWriteOnlyRepository<TEntity>> logger)
        {
            Db = db;
            AuthService = authService;
            _logger = logger;
        }

        #region Add methods
        public TEntity Add(TEntity entity, string[] excludeProperties = null)
        {
            SetCreateTrail(entity);
            Guard(entity, TimeSheetPermission._Create);
            try
            {
                GenerateCode(entity);
                Db.Add(entity);
                if (excludeProperties != null && excludeProperties.Any())
                {
                    foreach (var property in excludeProperties)
                    {
                        Db.Entry(entity).Property(property).IsModified = false;
                    }
                }
                return entity;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public TEntity AddEntityNotGuard(TEntity entity)
        {
            SetCreateTrail(entity);
            try
            {
                GenerateCode(entity);
                Db.Add(entity);
                return entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public List<TEntity> BatchAdd(List<TEntity> entities, string[] excludeProperties = null)
        {
            try
            {
                foreach (var entity in entities)
                {
                    SetCreateTrail(entity);
                    Guard(entity, TimeSheetPermission._Create);

                }

                GenerateRangeNextCode(entities);
                Db.AddRange(entities);
                if (excludeProperties != null && excludeProperties.Any())
                {
                    foreach (var entity in entities)
                    {
                        foreach (var property in excludeProperties)
                        {
                            Db.Entry(entity).Property(property).IsModified = false;
                        }
                    }
                }
                return entities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public List<TEntity> BatchAddNotGuard(List<TEntity> entities)
        {
            try
            {
                foreach (var entity in entities)
                {
                    SetCreateTrail(entity);
                }

                GenerateRangeNextCode(entities);
                Db.AddRange(entities);
                return entities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        #endregion

        #region Update methods
        public void Update(TEntity entity, string[] excludeProperties = null)
        {
            SetUpdateTrail(entity);
            Guard(entity, TimeSheetPermission._Update);
            try
            {
                GenerateCode(entity);
                Db.Entry(entity).State = EntityState.Modified;
                if (excludeProperties != null && excludeProperties.Any())
                {
                    foreach (var property in excludeProperties)
                    {
                        Db.Entry(entity).Property(property).IsModified = false;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public void UpdateEntityNotGuard(TEntity entity)
        {
            SetUpdateTrail(entity);
            try
            {
                GenerateCode(entity);
                Db.Entry(entity).State = EntityState.Modified;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public void BatchUpdate(List<TEntity> entities, string[] excludeProperties = null)
        {
            try
            {
                foreach (var entity in entities)
                {
                    SetUpdateTrail(entity);
                    Guard(entity, TimeSheetPermission._Update);

                }

                GenerateRangeNextCode(entities);
                Db.UpdateRange(entities);
                if (excludeProperties != null && excludeProperties.Any())
                {
                    foreach (var entity in entities)
                    {
                        foreach (var property in excludeProperties)
                        {
                            Db.Entry(entity).Property(property).IsModified = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
        #endregion

        #region Delete Methods
        public void Delete(TEntity entity)
        {
            if (IsSoftDelete())
            {
                SoftDelete(entity);
            }
            else if (IsSoftDeleteV2())
            {
                SoftDeleteV2(entity);
            }

            else
            {
                Guard(entity, TimeSheetPermission._Delete);
                try
                {
                    Db.Entry(entity).State = EntityState.Deleted;
                    Db.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    throw;
                }
            }
        }

        public void BatchDelete(List<TEntity> entities)
        {
            if (IsSoftDelete())
            {
                BatchSoftDelete(entities);
            }
            else
            {
                try
                {

                    foreach (var entity in entities)
                    {
                        Guard(entity, TimeSheetPermission._Delete);
                    }

                    Db.RemoveRange(entities);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    throw;
                }
            }
        }

        public void DisabledSoftDeleteFilter()
        {
            AuthService.WithDeleted = true;
        }
        #endregion

        #region Detach methods
        public void Detach(TEntity entity)
        {
            if (entity == null) return;
            Db.Entry(entity).State = EntityState.Detached;
        }

        public void BatchDetach(List<TEntity> entities)
        {
            if (entities == null || !entities.Any()) return;
            foreach (var entity in entities)
            {
                Detach(entity);
            }
        }

        public TEntity DetachByClone(TEntity entity, string[] includes = null)
        {
            return (TEntity)DetachByClone(entity, typeof(TEntity), includes);
        }
        public IList<TEntity> BatchDetachByClone(IList<TEntity> entities, string[] includes = null)
        {
            List<TEntity> detachedEntities = new List<TEntity>();
            var type = typeof(TEntity);
            foreach (var e in entities)
            {
                detachedEntities.Add((TEntity)DetachByClone(e, type, includes));
            }
            return detachedEntities;
        }
        #endregion

        #region Find and Get methods

        public async Task<TEntity> FindBySpecificationAsync(ISpecification<TEntity> spec, string include = null)
        {
            var queryGetAll = await GetAllAsync();
            if (!string.IsNullOrEmpty(include))
                return await queryGetAll.Where(spec.GetExpression()).Include(include).FirstOrDefaultAsync();
            return await queryGetAll.Where(spec.GetExpression()).FirstOrDefaultAsync();
        }

        public async Task<TEntity> FindBySpecificationWithIncludesAsync(ISpecification<TEntity> spec, List<string> includes = null)
        {
            var queryGetAll = await GetAllAsync();
            if (includes == null || !includes.Any())
                return await queryGetAll.Where(spec.GetExpression()).FirstOrDefaultAsync();

            var queryEntity = queryGetAll.Where(spec.GetExpression());
            foreach (var include in includes)
            {
                queryEntity = queryEntity.Include(include);
            }

            return await queryEntity.FirstOrDefaultAsync();

        }

        public async Task<TEntity> FindByIdAsync(object id)
        {
            var spec = GetFindByPrimaryKeySpec((long)id);
            return await FindBySpecificationAsync(spec);
        }

        public async Task<List<TEntity>> GetBySpecificationAsync(ISpecification<TEntity> spec, string include = null)
        {
            if (!string.IsNullOrEmpty(include))
                return await (await GetAllAsync()).Where(spec.GetExpression()).Include(include).ToListAsync();
            return await (await GetAllAsync()).Where(spec.GetExpression()).ToListAsync();
        }

        public async Task<long> CountByExpressionAsync(Expression<Func<TEntity, bool>> filter)
        {
            return await (await GetAllAsync()).CountAsync(filter);
        }

        public async Task<long> CountBySpecificationAsync(ISpecification<TEntity> spec)
        {
            return await (await GetAllAsync()).CountAsync(spec.GetExpression());
        }

        public async Task<bool> AnyBySpecificationAsync(ISpecification<TEntity> spec)
        {
            return await (await GetAllAsync()).AnyAsync(spec.GetExpression());
        }
        #endregion

        public void GenerateRangeNextCode(List<TEntity> entities)
        {
            var entitiesDoNotHaveCode = entities.Where(e => string.IsNullOrWhiteSpace((e as ICode)?.Code)).ToList();
            var entitiesHaveCode = entities.Where(e => !string.IsNullOrWhiteSpace((e as ICode)?.Code)).ToList();

            var codePrefix = typeof(TEntity).GetField("CodePrefix", BindingFlags.Public | BindingFlags.Static);
            var codeDraftSuffix = typeof(TEntity).GetField("CodeDraftSuffix", BindingFlags.Public | BindingFlags.Static);
            var isDraft = (entitiesDoNotHaveCode.FirstOrDefault() as IIsDraft)?.IsDraft ?? false;

            if (codePrefix == null || (isDraft && codeDraftSuffix == null)) return;

            var prefix = codePrefix.GetValue(typeof(TEntity)) as string;
            var draftSuffix = isDraft ? codeDraftSuffix.GetValue(typeof(TEntity)) as string : string.Empty;

            var latestCodedEntity = GetTEntity(prefix, draftSuffix, isDraft);

            var latestCode = (latestCodedEntity as ICode)?.Code;
            if (entitiesHaveCode.Any())
                latestCode = GetLatestCode(entitiesHaveCode, latestCode, prefix);

            if (!entitiesDoNotHaveCode.Any()) return;

            var listCode = new List<string>();

            entitiesDoNotHaveCode.ForEach(e =>
            {
                var code = _getNextCode(latestCode, prefix, CodePad, draftSuffix);

                listCode.Add(code);
                latestCode = code;
            });

            if (listCode == null || !listCode.Any())
            {
                var exUniqueMultipleCodes = new Exception("Invalid code: empty");
                _logger.LogError(exUniqueMultipleCodes, exUniqueMultipleCodes.Message);
                throw exUniqueMultipleCodes;
            }

            for (var i = 0; i < entitiesDoNotHaveCode.Count; i++)
            {
                ((ICode)entitiesDoNotHaveCode[i]).Code = listCode[i];
            }

        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Db?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region Private methods
        private async Task<IQueryable<TEntity>> GetAllAsync()
        {
            var hasPermissionBranch = AuthService.Context.User.IsAdmin;
            IList<int> authorizedBranchIds = new List<int>();
            if (!hasPermissionBranch)
            {
                authorizedBranchIds = await AuthService.GetAuthorizedBranchIds();
                hasPermissionBranch = AuthService.HasPermittedOnBranch(AuthService.Context.BranchId, authorizedBranchIds);
            }
            
            var query = Db.Set<TEntity>() as IQueryable<TEntity>;
            if (typeof(TEntity).HasInterface(typeof(ITenantId)))
                query = query.Where(e => EF.Property<int>(e, "TenantId") == AuthService.Context.TenantId);
            if (typeof(TEntity).HasInterface(typeof(IBranchId)))
                query = query.Where(e =>
                    hasPermissionBranch ||
                    authorizedBranchIds.Any(b => b == EF.Property<int>(e, "BranchId")) ||
                    e.GetType() != typeof(BranchSetting)
                );
            if (typeof(TEntity).HasInterface(typeof(ISoftDelete)))
                query = query.Where(e => AuthService.WithDeleted || !EF.Property<bool>(e, "IsDeleted"));
            return query;
        }

        private void SetCreateTrail(TEntity entity)
        {
            if (entity is ITenantId tenantEntity)
                tenantEntity.TenantId = AuthService.Context.TenantId;

            if (entity is IBranchId branchEntity && branchEntity.BranchId == 0)
                branchEntity.BranchId = AuthService.Context.BranchId;

            if (entity is ICreatedBy createdByEntity)
                createdByEntity.CreatedBy = AuthService.Context.User.Id;

            SetCreateOrUpdateTrailForChildren(entity);
        }

        private void SetUpdateTrail(TEntity entity)
        {
            if (entity is IModifiedDate auditRow)
                auditRow.ModifiedDate = DateTime.Now;

            if (entity is IModifiedBy modifyByRow)
                modifyByRow.ModifiedBy = AuthService.Context.User.Id;

            SetCreateOrUpdateTrailForChildren(entity);
        }

        private void SetCreateOrUpdateTrailForChildren(TEntity entity)
        {
            var properties = entity.GetType().GetProperties();
            foreach (var property in properties)
            {
                if(!HasChild(property)) continue;

                var childList = property.GetValue(entity);
                if (childList == null)
                    continue;

                SetCreateOrUpdate(childList);
            }
        }

        private static bool HasChild(PropertyInfo property)
        {
            if (!property.CanRead) return false;

            if (!(property.PropertyType.Namespace?.IndexOf("Collection") >= 0)) return false;

            var child = property.PropertyType.GetGenericArguments().FirstOrDefault();

            return child != null && Regex.IsMatch(child.Namespace ?? string.Empty,
                       "KiotVietTimeSheet.Domain.AggregatesModels.*.Models");
        }

        private void SetCreateOrUpdate(object childList)
        {
            foreach (var item in (IEnumerable)childList)
            {
                if (item is IEntityIdlong idChildEntity && idChildEntity.Id > 0)
                    SetModifiedDateAndModifiedBy(item);
                else
                    SetTenantBranchAndCreate(item);
            }
        }

        private void SetModifiedDateAndModifiedBy(object item)
        {
            if (item is IModifiedDate modifiedDateChildEntity)
                modifiedDateChildEntity.ModifiedDate = DateTime.Now;

            if (item is IModifiedBy modifyByChildEntity)
                modifyByChildEntity.ModifiedBy = AuthService.Context.User.Id;
        }

        private void SetTenantBranchAndCreate(object item)
        {
            if (item is ITenantId tenantChildEntity)
                tenantChildEntity.TenantId = AuthService.Context.TenantId;

            if (item is IBranchId branchChildEntity && branchChildEntity.BranchId == 0)
                branchChildEntity.BranchId = AuthService.Context.BranchId;

            if (item is ICreatedBy createdByChildEntity)
                createdByChildEntity.CreatedBy = AuthService.Context.User.Id;
        }

        private string GetNextCodeDelete(TEntity entity)
        {
            if (entity is ICode codeEntity)
            {
                var codeDelSuffixField = typeof(TEntity).GetField("CodeDelSuffix", BindingFlags.Public | BindingFlags.Static);
                if (codeDelSuffixField == null) return string.Empty;
                var codeDelSuffix = codeDelSuffixField.GetValue(typeof(TEntity)) as string;
                var tempDelCode = $"{codeEntity.Code}{codeDelSuffix}";

                var lsDeleted = Db.Set<TEntity>().IgnoreQueryFilters().Where(e =>
                    (e as ITenantId).TenantId == AuthService.Context.TenantId &&
                    (e as ISoftDelete).IsDeleted &&
                    (e as ICode).Code.Contains(tempDelCode)).ToList();

                if (lsDeleted == null || !lsDeleted.Any())
                    return codeEntity.Code + codeDelSuffix + "}";

                var lastCode = lsDeleted.Where(e =>
                                    (e as ICode).Code.Substring(0,
                                       (e as ICode).Code.IndexOf(codeDelSuffix, StringComparison.Ordinal) > -1 ? (e as ICode).Code.IndexOf(codeDelSuffix, StringComparison.Ordinal) : 0
                                    )
                                    .Equals(codeEntity.Code, StringComparison.OrdinalIgnoreCase)
                                )
                                .Select(v => ConvertHelper.ToInt32((v as ICode).Code.Substring((v as ICode).Code.IndexOf(codeDelSuffix, StringComparison.Ordinal) > -1
                    ? (v as ICode).Code.LastIndexOf(codeDelSuffix, StringComparison.Ordinal) + codeDelSuffix.Length
                    : (v as ICode).Code.Length).TrimEnd('}')))
                    .OrderByDescending(o => o)
                    .FirstOrDefault();

                var suffix = lastCode < 0 ? codeDelSuffix : $"{codeDelSuffix}{lastCode + 1}";
                return (entity as ICode).Code + suffix + "}";
            }

            return string.Empty;
        }

        private void PreProcessSoftDelete(TEntity entity)
        {
            ((ISoftDelete)entity).IsDeleted = true;
            ((ISoftDelete)entity).DeletedDate = DateTime.Now;
            ((ISoftDelete)entity).DeletedBy = AuthService.Context.User.Id;

            if (typeof(TEntity).HasInterface(typeof(ICode)))
            {
                var delCode = GetNextCodeDelete(entity);
                ((ICode)entity).Code = delCode;
            }
            if (typeof(TEntity).HasInterface(typeof(IName)))
            {
                ((IName)entity).Name += "{DEL}";
            }
        }

        private bool IsSoftDelete()
        {
            if (typeof(TEntity).HasInterface(typeof(ISoftDelete)))
                return true;

            return false;
        }
        private void PreProcessSoftDeleteV2(TEntity entity)
        {
            ((IModifiedBy)entity).ModifiedBy = AuthService.Context.User.Id;
            ((IModifiedDate)entity).ModifiedDate = DateTime.Now;
            if (typeof(TEntity).HasInterface(typeof(ICode)))
            {
                var delCode = GetNextCodeDelete(entity);
                ((ICode)entity).Code = delCode;
            }
            if (typeof(TEntity).HasInterface(typeof(IName)))
            {
                ((IName)entity).Name += "{DEL}";
            }
        }

        private bool IsSoftDeleteV2()
        {
            if (typeof(TEntity).HasInterface(typeof(ISoftDeleteV2)))
                return true;

            return false;
        }

        private void SoftDeleteV2(TEntity entity)
        {
            if (entity != null)
            {
                try
                {
                    PreProcessSoftDeleteV2(entity);
                    Update(entity);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    throw;
                }
            }
        }

        private void SoftDelete(TEntity entity)
        {
            if (entity != null)
            {
                try
                {
                    PreProcessSoftDelete(entity);
                    Update(entity);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    throw;
                }
            }
        }

        private void BatchSoftDelete(List<TEntity> entities)
        {
            if (entities == null || !entities.Any()) return;

            try
            {
                foreach (var entity in entities)
                {
                    PreProcessSoftDelete(entity);
                }
                BatchUpdate(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        private string GetRandomCode()
        {
            var random = new Random();
            var builder = new StringBuilder(10);
            for (int i = 0; i < 10; i++)
                builder.Append((char)random.Next(0x41, 0x5A));
            return builder.ToString().ToUpper();
        }

        private string _getNextCode(string previous, string prefix, int padd, string suffix)
        {
            long temp = 1;
            if (!string.IsNullOrWhiteSpace(previous))
            {
                Match match = Regex.Match(previous, "[0-9]+", RegexOptions.None);
                if (match.Success)
                {
                    long code = long.Parse(match.Groups[0].Value);
                    temp = code + 1;
                }
                else
                {
                    var ex = new Exception("Có lỗi xảy ra trong quá trình tạo mã");
                    _logger.LogError(ex, ex.Message);
                    throw ex;
                }
            }
            var res = $"{prefix}{temp.ToString().PadLeft(padd, '0')}{suffix}";
            return res;
        }

        private string GetPrefixStringByTypeTEntity(Type type)
        {
            var prefix = string.Empty;
            var codePrefix = type.GetField("CodePrefix", BindingFlags.Public | BindingFlags.Static);
            if (codePrefix != null)
            {
                prefix = codePrefix.GetValue(type) as string;
            }

            return prefix;
        }

        private string GetNextCode(TEntity entity)
        {
            if (!(entity is ICode codeEntity)) return string.Empty;

            var codePrefix = typeof(TEntity).GetField("CodePrefix", BindingFlags.Public | BindingFlags.Static);
            var codeDraftSuffix = typeof(TEntity).GetField("CodeDraftSuffix", BindingFlags.Public | BindingFlags.Static);
            var isDraft = (entity as IIsDraft)?.IsDraft ?? false;

            if (codePrefix == null || (isDraft && codeDraftSuffix == null)) return string.Empty;

            var prefix = codePrefix.GetValue(typeof(TEntity)) as string;
            var draftSuffix = isDraft ? codeDraftSuffix.GetValue(typeof(TEntity)) as string : string.Empty;


            if (!string.IsNullOrWhiteSpace(codeEntity.Code)) return string.Empty;

            var latestCodedEntity = GetTEntity(prefix, draftSuffix, isDraft);
            return _getNextCode((latestCodedEntity as ICode)?.Code, prefix, CodePad, draftSuffix);
        }

        private string CheckUniqueCode(string suggest)
        {
            if (string.IsNullOrWhiteSpace(suggest))
            {
                var ex = new Exception("Invalid code: empty");
                _logger.LogError(ex, ex.Message);
                throw ex;
            }
            var type = typeof(TEntity);
            if (type.GetInterface("ICode") == null) return string.Empty;

            var prefix = GetPrefixStringByTypeTEntity(type);

            var limitByTenantIdSpec = new LimitByTenantIdSpec<TEntity>(AuthService.Context.TenantId);
            var checkExistsCodeSpec = new CheckExistsCodeSpec<TEntity>(suggest);

            var query = Db.Set<TEntity>()
                .Where(limitByTenantIdSpec.GetExpression())
                .Where(checkExistsCodeSpec.GetExpression());

            var exists = query.Any();
            return exists ? prefix + "-" + GetRandomCode() : suggest;
        }

        private void GenerateCode(TEntity entity)
        {
            if (!(entity is ICode codedEntity)) return;

            if (!string.IsNullOrWhiteSpace(codedEntity.Code)) return;

            var candidate = GetNextCode(entity);
            if (candidate != null)
                codedEntity.Code = CheckUniqueCode(candidate);
        }
    

        private void Guard(TEntity entity, string permission)
        {
            Guard(entity, new[] { permission });
        }

        private void Guard(TEntity entity, string[] permissions)
        {
            var type = typeof(TEntity);
            for (int i = 0; i < permissions.Length; i++)
            {
                permissions[i] = type.Name + permissions[i];
            }

            if (type.HasInterface(typeof(ITenantId)))
            {
                var tenantEntity = (entity as ITenantId);
                if (tenantEntity?.TenantId == 0)
                    throw new KvTimeSheetTenantNotFoundException();

                if (tenantEntity?.TenantId != AuthService.Context.TenantId)
                    throw new KvTimeSheetUnAuthorizedException("Bạn không có quyền thực hiện");
            }

            if (!type.HasInterface(typeof(IBranchId))) return;

            var branchEntity = (entity as IBranchId);
            if (branchEntity?.BranchId == 0)
                throw new KvTimeSheetBranchNotFoundException();
        }

        private ISpecification<TEntity> GetFindByPrimaryKeySpec(long key)
        {
            if (typeof(TEntity).HasInterface(typeof(IEntityIdlong)))
            {
                return new FindByEntityIdLongSpec<TEntity>(key);
            }

            var ex = new Exception("Can't find spec matched primary key");
            _logger.LogError(ex, ex.Message);
            throw ex;
        }

        private object DetachByClone(object entity, Type type, string[] includes = null)
        {
            var constructor = type.GetConstructor(new[] { type });
            if (constructor == null)
            {
                var ex = new KvTimeSheetException(Message.generalErrorMsg);
                _logger.LogError(ex, ex.Message);
                throw ex;
            }

            var clone = constructor.Invoke(new[] { entity });
            if (includes == null) return clone;

            foreach (var p in includes)
            {
                var prop = type.GetProperty(p);
                if (prop == null || !prop.CanRead) continue;

                var buffer = prop.GetValue(entity);
                if (buffer == null) continue;

                var indexCollection = prop.PropertyType.Namespace?.IndexOf("Collection");

                if (indexCollection >= 0)
                    SetValuePropertyInfo(prop, buffer, clone);
                else if (Regex.IsMatch(prop.PropertyType.Namespace ?? string.Empty,
                    "KiotVietTimeSheet.Domain.AggregatesModels.*.Models"))
                {
                    var copy = prop.PropertyType.GetConstructor(new[] {prop.PropertyType})?.Invoke(new[] {buffer});
                    if (copy != null) prop.SetValue(clone, copy);
                }
                else
                    prop.SetValue(clone, buffer);
            }
            return clone;
        }

        private TEntity GetTEntity(string prefix, string draftSuffix, bool isDraft)
        {
            var limitByTenantIdSpec = new LimitByTenantIdSpec<TEntity>(AuthService.Context.TenantId);
            var startWithCodeSpec = new StartWithCodePrefixSpec<TEntity>(prefix);
            var limitByCodeLengthSpec = new LimitByCodeLengthSpec<TEntity>((prefix?.Length ?? 0) + CodePad + (draftSuffix?.Length ?? 0));
            var checkCodeIsValidSpec = new CheckCodeIsValidSpec<TEntity>((prefix?.Length ?? 0) + CodePad + (draftSuffix?.Length ?? 0), prefix, draftSuffix);

            ISpecification<TEntity> endWithCodeSpec = new DefaultTrueSpec<TEntity>();
            if (isDraft)
            {
                endWithCodeSpec = new EndWithCodePrefixSpec<TEntity>(draftSuffix);
            }

            // Tạm xài như vậy
            var query = Db.Set<TEntity>()
                .Where(limitByTenantIdSpec.GetExpression())
                .Where(startWithCodeSpec.GetExpression())
                .Where(limitByCodeLengthSpec.GetExpression())
                .Where(checkCodeIsValidSpec.GetExpression())
                .Where(endWithCodeSpec.GetExpression())
                // .Where(Sql.Custom($"ISNUMERIC(SUBSTRING(Code, { prefix.Length + 1 }, LEN(Code) - {prefix.Length})) = 1"))
                .OrderByDescending(e => (e as ICode).Code);

            var latestCodedEntity = query.FirstOrDefault();

            return latestCodedEntity;
        }

        private static string GetLatestCode(IEnumerable<TEntity> entitiesHaveCode, string latestCode, string prefix)
        {
            var codes = entitiesHaveCode.Where(e => ((ICode)e).Code.StartsWith(prefix)).ToList();

            if (!codes.Any()) return latestCode;

            var validCodes = codes.Select(e =>
                    {
                        var code = (e as ICode)?.Code;
                        var identityCode = code?.Remove(0, prefix?.Length ?? 0);
                        if (long.TryParse(identityCode, out var identity))
                        {
                            return identity;
                        }

                        return -1;
                    })
                    .Where(c => c > 0).ToList();


            if (!validCodes.Any()) return latestCode;

            var maxValidCode = validCodes.Max();

            if (latestCode != null)
            {
                var serverLatestCode = latestCode.Remove(0, prefix?.Length ?? 0);
                if (long.TryParse(serverLatestCode, out var max))
                {
                    latestCode = max > maxValidCode ? max.ToString() : maxValidCode.ToString();
                }
            }
            else
            {
                latestCode = maxValidCode.ToString();
            }

            return latestCode;
        }

        private static void SetValuePropertyInfo(PropertyInfo prop, object buffer, object clone)
        {
            var child = prop.PropertyType.GetGenericArguments().FirstOrDefault();

            if (child == null || !Regex.IsMatch(child.Namespace ?? string.Empty,
                    "KiotVietTimeSheet.Domain.AggregatesModels.*.Models")) return;

            var list = (IList)typeof(List<>).MakeGenericType(child).GetConstructor(Type.EmptyTypes)?.Invoke(null);
            var childConstructor = child.GetConstructor(new[] { child });

            if (list != null && childConstructor != null)
            {
                foreach (var o in (IEnumerable) buffer)
                {
                    list.Add(childConstructor.Invoke(new[] { o }));
                }
            }
            prop.SetValue(clone, list);
        }
        #endregion
    }
}
