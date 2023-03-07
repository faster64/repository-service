using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;
using KiotVietTimeSheet.Domain.AggregatesModels.CommissionDetailAggregate.Enums;
using KiotVietTimeSheet.Domain.Common;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions;
using KiotVietTimeSheet.Domain.Engines.SalaryEngine.Objects;
using Newtonsoft.Json;
using ServiceStack;

namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Rules.CommisisonSalaryV2
{
    public class CommissionSalaryRuleV2 : BaseRule<CommissionSalaryRuleValueV2, CommissionSalaryRuleParamV2>
    {
        private readonly List<ValidationFailure> _errors;

        public CommissionSalaryRuleV2(CommissionSalaryRuleValueV2 value, CommissionSalaryRuleParamV2 param)
            : base(value, param)
        {
            _errors = new List<ValidationFailure>();
        }

        public override List<ValidationFailure> Errors => _errors;

        public override decimal Process()
        {
            if (Param?.CommissionParams == null || !Param.CommissionParams.Any()) return 0;

            switch (Value.Type)
            {
                case CommissionSalaryTypes.WithTotalCommission:
                    return ProcessRuleWithTotalCommission();
                case CommissionSalaryTypes.WithLevelCommission:
                    return ProcessRuleWithLevelCommission();
                case CommissionSalaryTypes.WithMinimumCommission:
                    return ProcessRuleWithMinimumCommission();
                case CommissionSalaryTypes.WithTotalPersonalGrossProfit:
                    return ProcessRuleWithTotalPersonalGrossProfit();
                default:
                    return 0;
            }
        }

        public override void Factory(EngineResource resource)
        {
            if (Value.CommissionSalaryRuleValueDetails == null || !Value.CommissionSalaryRuleValueDetails.Any())
            {
                Param = new CommissionSalaryRuleParamV2();
                return;
            }

            switch (Value.Type)
            {
                case CommissionSalaryTypes.WithTotalCommission:
                    Param = InitParamsRuleWithTotalCommission(resource);
                    break;
                case CommissionSalaryTypes.WithLevelCommission:
                    Param = InitParamsRuleWithLevelCommission(resource);
                    break;
                case CommissionSalaryTypes.WithMinimumCommission:
                    Param = InitParamsRuleWithMinimumCommission(resource);
                    break;
                case CommissionSalaryTypes.WithTotalPersonalGrossProfit:
                    Param = InitParamsRuleWithTotalPersonalGrossProfit(resource);
                    break;
                default:
                    Param = new CommissionSalaryRuleParamV2(); break;
            }
            Param.CommissionSalaryOrigin = Process();
        }

        public override void Init()
        {

        }

        public override void UpdateParam(string ruleParam)
        {
            if (string.IsNullOrEmpty(ruleParam)) return;
            var oldRuleParam = JsonConvert.DeserializeObject(ruleParam, typeof(CommissionSalaryRuleParamV2)) as CommissionSalaryRuleParamV2;
            if (Param == null || oldRuleParam == null) return;
            if (Value.Type != oldRuleParam.Type)
            {
                Param.CommissionSalary = Process();
                return;
            }
            switch (Value.Type)
            {
                case CommissionSalaryTypes.WithTotalCommission:
                    UpdateParamsWithTotalCommission(oldRuleParam); break;
                case CommissionSalaryTypes.WithMinimumCommission:
                    UpdateParamsWithMinimumCommission(oldRuleParam); break;
                case CommissionSalaryTypes.WithLevelCommission:
                    UpdateParamsWithLevelCommission(oldRuleParam); break;
                case CommissionSalaryTypes.WithTotalPersonalGrossProfit:
                    UpdateParamsWithPersonalGrossProfit(oldRuleParam); break;
                default:
                    var ex = new Exception("Không xác định kiểu lương hoa hồng");
                    throw ex;
            }
        }

        public override bool IsValid()
        {
            var validateValueResult = new CommissionSalaryRuleValueValidatorV2().Validate(Value);
            if (!validateValueResult.IsValid)
            {
                _errors.AddRange(validateValueResult.Errors);
            }
            return !_errors.Any();
        }

        public override bool IsEqual(IRule rule)
        {
            if (rule == null) return false;
            if (Value == null && rule.GetRuleValue() == null) return true;
            return Value != null && Value.IsEqual(rule.GetRuleValue() as CommissionSalaryRuleValueV2);
        }

        #region Private methods

        private CommissionSalaryRuleParamV2 InitParamsRuleWithTotalCommission(EngineResource resource)
        {
            var totalRevenue = InitTotalRevenueCommission(resource);
            //tổng doanh thu tư vấn
            var totalCounselorRevenue = resource.TotalCounselorRevenue;
            Value.MinCommission = Value.MinCommission ?? 0;

            //rule hoa hồng dịch vụ
            var ruleServiceDetailApply = Value.CommissionSalaryRuleValueDetails
                .Where(x => x.CommissionType == (byte)CommissionType.Service || x.CommissionType == 0)
                .OrderByDescending(x => x.CommissionLevel ?? -1)
                .FirstOrDefault(x => x.CommissionLevel < totalRevenue || (totalRevenue == 0 && x.CommissionLevel == 0));

            //rule hoa hồng tư vấn
            var ruleCounselorDetailApply = Value.CommissionSalaryRuleValueDetails
                .Where(x => x.CommissionType == (byte)CommissionType.Counselor)
                .OrderByDescending(x => x.CommissionLevel ?? -1)
                .FirstOrDefault(x => x.CommissionLevel < totalCounselorRevenue || (totalCounselorRevenue == 0 && x.CommissionLevel == 0));

            var productServiceRevenues = ruleServiceDetailApply?.CommissionTableId != null
                ? InitProductRevenuesCommission(resource, (byte)CommissionType.Service)
                : new List<ProductRevenue>();

            productServiceRevenues = productServiceRevenues.Where(x => x.Quantity > 0).ToList();

            var commissionServiceTable = ruleServiceDetailApply?.CommissionTableId != null
                ? resource.Commissions.FirstOrDefault(x => x.Id == ruleServiceDetailApply.CommissionTableId)?.CreateCopy()
                : null;

            var productCounselorRevenues = ruleCounselorDetailApply?.CommissionTableId != null
                ? InitProductRevenuesCommission(resource, (byte)CommissionType.Counselor)
                : new List<ProductRevenue>();

            productCounselorRevenues = productCounselorRevenues.Where(x => x.Quantity > 0).ToList();

            var commissionCounselorTable = ruleCounselorDetailApply?.CommissionTableId != null
                ? resource.Commissions.FirstOrDefault(x => x.Id == ruleCounselorDetailApply.CommissionTableId)?.CreateCopy()
                : null;

            if (commissionServiceTable != null)
            {
                commissionServiceTable.CommissionDetails = commissionServiceTable.CommissionDetails ?? new List<CommissionTableDetailParam>();
                commissionServiceTable.CommissionDetails = commissionServiceTable.CommissionDetails
                    .Where(x => !x.IsDeleted && productServiceRevenues.Any(pv =>
                    (
                        pv.ProductId == x.ObjectId && x.Type == (byte)ObjectType.Product)
                        || (!string.IsNullOrEmpty(pv.CategoryIds) && pv.CategoryIds.Split(',').Select(long.Parse).ToList().Contains(x.ObjectId) && x.Type == (byte)ObjectType.Category)
                    ))
                    .Select(x =>
                    {
                        return new CommissionTableDetailParam(x.Id, x.ObjectId, x.Value, x.ValueRatio, x.Type, false);
                    }).ToList();
            }

            var commissionServiceParam = new CommissionParam()
            {
                CommissionType = (byte)CommissionType.Service,
                Value = ruleServiceDetailApply?.Value,
                ValueRatio = ruleServiceDetailApply?.ValueRatio,
                ValueOrigin = ruleServiceDetailApply?.Value,
                ValueRatioOrigin = ruleServiceDetailApply?.ValueRatio,
                CommissionLevel = ruleServiceDetailApply?.CommissionLevel ?? 0,
                ProductRevenues = productServiceRevenues,
                CommissionTable = commissionServiceTable,
                CommissionSetting = resource.SettingsToObject?.CommissionSetting ?? 0
            };

            if (commissionCounselorTable != null)
            {
                commissionCounselorTable.CommissionDetails = commissionCounselorTable.CommissionDetails ?? new List<CommissionTableDetailParam>();
                commissionCounselorTable.CommissionDetails = commissionCounselorTable.CommissionDetails
                    .Where(x => !x.IsDeleted && productCounselorRevenues.Any(pv =>
                    (
                        pv.ProductId == x.ObjectId && x.Type == (byte)ObjectType.Product)
                        || (!string.IsNullOrEmpty(pv.CategoryIds) && pv.CategoryIds.Split(',').Select(long.Parse).ToList().Contains(x.ObjectId) && x.Type == (byte)ObjectType.Category)
                    ))
                    .Select(x =>
                    {
                        return new CommissionTableDetailParam(x.Id, x.ObjectId, x.Value, x.ValueRatio, x.Type, false);
                    }).ToList();
            }

            var commissionCounselorParam = new CommissionParam()
            {
                CommissionType = (byte)CommissionType.Counselor,
                Value = ruleCounselorDetailApply?.Value,
                ValueRatio = ruleCounselorDetailApply?.ValueRatio,
                ValueOrigin = ruleCounselorDetailApply?.Value,
                ValueRatioOrigin = ruleCounselorDetailApply?.ValueRatio,
                CommissionLevel = ruleCounselorDetailApply?.CommissionLevel ?? 0,
                ProductRevenues = productCounselorRevenues,
                CommissionTable = commissionCounselorTable
            };

            var ruleParams = new CommissionSalaryRuleParamV2
            {
                Type = CommissionSalaryTypes.WithTotalCommission,
                TotalRevenue = totalRevenue,
                TotalCounselorRevenue = totalCounselorRevenue,
                CommissionParams = new List<CommissionParam> { commissionServiceParam, commissionCounselorParam }
            };

            return ruleParams;
        }

        private CommissionSalaryRuleParamV2 InitParamsRuleWithLevelCommission(EngineResource resource)
        {
            var totalRevenue = InitTotalRevenueCommission(resource);
            var commissionParams = Value.CommissionSalaryRuleValueDetails
                .OrderBy(x => x.CommissionLevel ?? -1)
                .Select(ruleDetail =>
                {
                    var commissionParam = new CommissionParam()
                    {
                        Value = ruleDetail.Value,
                        ValueRatio = ruleDetail.ValueRatio,
                        ValueOrigin = ruleDetail.Value,
                        ValueRatioOrigin = ruleDetail.ValueRatio,
                        CommissionLevel = ruleDetail.CommissionLevel == 0 ? null : ruleDetail.CommissionLevel
                    };
                    return commissionParam;
                })
                .ToList();

            if (Value.CommissionSalaryRuleValueDetails.All(x => x.CommissionLevel != 0))
            {
                commissionParams.Add(new CommissionParam
                {
                    CommissionLevel = null,
                    CommissionTable = null,
                    Value = null,
                    IsDirty = false,
                    ValueRatio = 0,
                    ProductRevenues = null,
                    ValueOrigin = null,
                    ValueRatioOrigin = 0
                });
            }

            var ruleParams = new CommissionSalaryRuleParamV2
            {
                Type = CommissionSalaryTypes.WithLevelCommission,
                TotalRevenue = totalRevenue,
                CommissionParams = commissionParams
            };

            return ruleParams;
        }

        private CommissionSalaryRuleParamV2 InitParamsRuleWithMinimumCommission(EngineResource resource)
        {
            var totalRevenue = InitTotalRevenueCommission(resource);
            Value.MinCommission = Value.MinCommission ?? 0;
            var ruleDetailApply = Value.CommissionSalaryRuleValueDetails
                .OrderByDescending(x => x.CommissionLevel ?? -1)
                .FirstOrDefault(x => x.CommissionLevel + Value.MinCommission <= totalRevenue);

            var commissionParam = new CommissionParam()
            {
                Value = ruleDetailApply?.Value,
                ValueRatio = ruleDetailApply?.ValueRatio,
                ValueOrigin = ruleDetailApply?.Value,
                ValueRatioOrigin = ruleDetailApply?.ValueRatio,
                CommissionLevel = ruleDetailApply?.CommissionLevel ?? 0,
            };

            var ruleParams = new CommissionSalaryRuleParamV2
            {
                Type = CommissionSalaryTypes.WithMinimumCommission,
                TotalRevenue = totalRevenue,
                CommissionParams = new List<CommissionParam> { commissionParam }
            };

            return ruleParams;
        }

        private CommissionSalaryRuleParamV2 InitParamsRuleWithTotalPersonalGrossProfit(EngineResource resource)
        {
            var totalGrossProfit = InitTotalGrossProfitCommission(resource);
            CommissionSalaryRuleValueDetailV2 ruleDetailApply = null;
            if (totalGrossProfit >= 0)
            {
                ruleDetailApply = Value.CommissionSalaryRuleValueDetails
                .OrderByDescending(x => x.CommissionLevel ?? -1)
                .FirstOrDefault(x => x.CommissionLevel <= totalGrossProfit);
            }
            var commissionParam = new CommissionParam()
            {
                Value = ruleDetailApply?.Value,
                ValueRatio = ruleDetailApply?.ValueRatio,
                ValueOrigin = ruleDetailApply?.Value,
                ValueRatioOrigin = ruleDetailApply?.ValueRatio,
                CommissionLevel = ruleDetailApply?.CommissionLevel ?? 0,
            };

            var ruleParams = new CommissionSalaryRuleParamV2
            {
                Type = CommissionSalaryTypes.WithTotalPersonalGrossProfit,
                TotalGrossProfit = totalGrossProfit,
                CommissionParams = new List<CommissionParam> { commissionParam }
            };

            return ruleParams;
        }

        private decimal ProcessRuleWithTotalCommission()
        {
            decimal result = 0;
            decimal totalRevenue = 0;
            decimal totalCounselorRevenue = 0;
            #region tính hoa hồng dịch vụ
            var ruleServiceParamDetail = Param.CommissionParams.FirstOrDefault(x => x.CommissionType == (byte)CommissionType.Service);
            if (ruleServiceParamDetail?.ProductRevenues != null)
                totalRevenue = ruleServiceParamDetail.ProductRevenues.Sum((x => x.TotalCommission));

            if (ruleServiceParamDetail?.CommissionTable != null)
            {
                if (ruleServiceParamDetail.ProductRevenues != null)
                {
                    result += ruleServiceParamDetail.ProductRevenues
                    .Select(productRevenue =>
                    {
                        var commissionDetail = ruleServiceParamDetail.CommissionTable.CommissionDetails
                            .FirstOrDefault(x => x.ObjectId == productRevenue.ProductId && x.Type == (byte)ObjectType.Product);
                        // Nếu sp ko có trong bảng hh thì check hh theo nhóm hàng
                        if (commissionDetail == null && !string.IsNullOrEmpty(productRevenue.CategoryIds))
                        {
                            var applyCategoryId = GetCommissionDetailCategoryId(ruleServiceParamDetail, productRevenue);
                            if (applyCategoryId != 0)
                            {
                                commissionDetail = ruleServiceParamDetail.CommissionTable.CommissionDetails
                                                    .FirstOrDefault(c => c.ObjectId == applyCategoryId && c.Type == (byte)ObjectType.Category);
                            }
                        }
                        // Nếu thiết lập hoa hồng là chia đều thì chia theo đầu người
                        var totalEmployee = 1;
                        if (ruleServiceParamDetail.CommissionSetting == (int)CommissionSetting.EquallyShare
                            && productRevenue.TotalEmployee > 0)
                        {
                            totalEmployee = productRevenue.TotalEmployee;
                        }
                        if (commissionDetail?.ValueRatio != null)
                            return commissionDetail.ValueRatio.Value * productRevenue.TotalCommission / 100;
                        return (commissionDetail?.Value.GetValueOrDefault() ?? 0) * (decimal)productRevenue.Quantity / totalEmployee;
                    })
                    .Sum();
                }
            }
            else if (ruleServiceParamDetail?.ValueRatio != null)
                result += Param.TotalRevenue * ruleServiceParamDetail.ValueRatio.Value / 100;
            else
                result += ruleServiceParamDetail?.Value.GetValueOrDefault() ?? 0;
            #endregion
            #region tính hoa hồng tư vấn
            var ruleCounselorParamDetail = Param.CommissionParams.FirstOrDefault(x => x.CommissionType == (byte)CommissionType.Counselor);
            if (ruleCounselorParamDetail?.ProductRevenues != null)
                totalCounselorRevenue = ruleCounselorParamDetail.ProductRevenues.Sum((x => x.TotalCommission));

            if (ruleCounselorParamDetail?.CommissionTable != null)
            {
                if (ruleCounselorParamDetail.ProductRevenues != null)
                {
                    result += ruleCounselorParamDetail.ProductRevenues
                    .Select(productRevenue =>
                    {
                        var commissionDetail = ruleCounselorParamDetail.CommissionTable.CommissionDetails
                            .FirstOrDefault(x => x.ObjectId == productRevenue.ProductId && x.Type == (byte)ObjectType.Product);
                        // Nếu sp ko có trong bảng hh thì check hh theo nhóm hàng
                        if (commissionDetail == null && !string.IsNullOrEmpty(productRevenue.CategoryIds))
                        {
                            var applyCategoryId = GetCommissionDetailCategoryId(ruleCounselorParamDetail, productRevenue);
                            if (applyCategoryId != 0)
                            {
                                commissionDetail = ruleCounselorParamDetail.CommissionTable.CommissionDetails
                                                    .FirstOrDefault(c => c.ObjectId == applyCategoryId && c.Type == (byte)ObjectType.Category);
                            }
                        }
                        if (commissionDetail?.ValueRatio != null)
                            return commissionDetail.ValueRatio.Value * productRevenue.TotalCommission / 100;
                        return (commissionDetail?.Value.GetValueOrDefault() ?? 0) * (decimal)productRevenue.Quantity;
                    })
                    .Sum();
                }
            }
            else if (ruleCounselorParamDetail?.ValueRatio != null)
                result += Param.TotalCounselorRevenue * ruleCounselorParamDetail.ValueRatio.Value / 100;
            else
                result += ruleCounselorParamDetail?.Value.GetValueOrDefault() ?? 0;
            #endregion


            return Math.Round(result, 0);
        }

        private decimal ProcessRuleWithLevelCommission()
        {
            decimal result = 0;
            var totalRevenue = Param.TotalRevenue;
            if (totalRevenue <= 0) return 0;
            var paramsOrdered = Param.CommissionParams
                .OrderByDescending(x => x.CommissionLevel ?? -1);

            foreach (var param in paramsOrdered)
            {
                if (param.CommissionLevel != null && totalRevenue <= param.CommissionLevel) continue;
                var commissionLevel = param.CommissionLevel.GetValueOrDefault();
                if (param.ValueRatio != null)
                    result += (totalRevenue - commissionLevel) * param.ValueRatio.Value / 100;
                else
                    result += param.Value.GetValueOrDefault();
                totalRevenue = commissionLevel;
            }

            return Math.Round(result, 0);
        }

        private decimal ProcessRuleWithMinimumCommission()
        {
            if (Param.TotalRevenue <= 0) return 0;
            var minCommission = Value.MinCommission.GetValueOrDefault();
            var revenueForCalculateSalary = Param.TotalRevenue > minCommission ? Param.TotalRevenue - minCommission : 0;
            var ruleParamDetail = Param.CommissionParams.FirstOrDefault();
            if (ruleParamDetail == null) return 0;
            var result = ruleParamDetail.ValueRatio * revenueForCalculateSalary / 100 ?? ruleParamDetail.Value.GetValueOrDefault();
            return Math.Round(result, 0);
        }
        //Tính hoa hồng theo lợi nhuận gộp mà nhân viên được nhận
        private decimal ProcessRuleWithTotalPersonalGrossProfit()
        {
            var ruleParamDetail = Param.CommissionParams.FirstOrDefault();
            if (ruleParamDetail == null) return 0;
            var result = ruleParamDetail.ValueRatio * Param.TotalGrossProfit / 100 ?? ruleParamDetail.Value.GetValueOrDefault();
            return Math.Round(result, 0);
        }

        private void UpdateParamsWithTotalCommission(CommissionSalaryRuleParamV2 oldRuleParam)
        {
            // Tham số mới (hoa hồng theo tổng doanh thu chỉ có 1 phần tử tham số)
            var updateParam = Param.CommissionParams.FirstOrDefault();
            if (updateParam == null) return;

            // Tham số cũ
            var oldParam = oldRuleParam.CommissionParams.FirstOrDefault();
            if (oldParam == null) return;

            if (updateParam.CommissionTable == null && oldParam.CommissionTable == null) return;

            // Nếu lương nhân viên thay đổi từ sử dụng bảng hoa hồng sang vnd/% hoặc ngược lại thì tính theo cài đặt mức lương của nhân viên đó
            if (oldParam.CommissionTable == null || updateParam.CommissionTable == null || updateParam.CommissionTable.Id != oldParam.CommissionTable.Id)
            {
                Param.CommissionParams.ForEach(x => x.IsDirty = false);
                Param.CommissionSalary = Process();
                return;
            }

            // Nếu nhân viên sử dụng bảng hoa hồng
            foreach (var commissionDetail in updateParam.CommissionTable.CommissionDetails)
            {
                var oldDetailExist = oldParam.CommissionTable
                    .CommissionDetails
                    .FirstOrDefault(x => x.ObjectId == commissionDetail.ObjectId);

                commissionDetail.IsDirty = !commissionDetail.IsDeleted && oldDetailExist != null &&
                                           (oldDetailExist.IsDirty.GetValueOrDefault()
                                            || oldDetailExist.ValueOrigin != oldDetailExist.Value
                                            || oldDetailExist.ValueRatioOrigin != oldDetailExist.ValueRatio
                                           );

                if (oldDetailExist == null || !commissionDetail.IsDirty.GetValueOrDefault()) continue;

                commissionDetail.Value = oldDetailExist.Value;
                commissionDetail.ValueRatio = oldDetailExist.ValueRatio;
            }


            Param.CommissionSalary = Process();

            if (updateParam.CommissionTable != null || oldParam.CommissionTable != null) return;

            // Nếu nhân viên sử dụng % hoặc vnđ
            updateParam.IsDirty = oldParam.IsDirty.GetValueOrDefault();
            if (!updateParam.IsDirty.GetValueOrDefault()) return;

            updateParam.Value = oldParam.Value;
            updateParam.ValueRatio = oldParam.ValueRatio;
            Param.CommissionSalary = Process();
        }

        private void UpdateParamsWithLevelCommission(CommissionSalaryRuleParamV2 oldRuleParam)
        {
            var updateParams = Param.CommissionParams;
            var oldParams = oldRuleParam.CommissionParams;

            if (updateParams.All(x => x.CommissionLevel != null))
            {
                updateParams.Add(new CommissionParam
                {
                    CommissionLevel = null,
                    CommissionTable = null,
                    Value = null,
                    IsDirty = false,
                    ValueRatio = 0,
                    ProductRevenues = null,
                    ValueOrigin = null,
                    ValueRatioOrigin = 0
                });
            }

            updateParams.ForEach(updateParam =>
            {
                var existOldParam = oldParams.FirstOrDefault(x => x.CommissionLevel == updateParam.CommissionLevel);
                updateParam.IsDirty = existOldParam != null && (existOldParam.IsDirty.GetValueOrDefault() || existOldParam.ValueOrigin != existOldParam.Value || existOldParam.ValueRatioOrigin != existOldParam.ValueRatio);
                if (existOldParam == null || !updateParam.IsDirty.GetValueOrDefault()) return;
                updateParam.Value = existOldParam.Value;
                updateParam.ValueRatio = existOldParam.ValueRatio;
            });

            Param.CommissionSalary = Process();
        }

        private void UpdateParamsWithMinimumCommission(CommissionSalaryRuleParamV2 oldRuleParam)
        {
            // Tham số mới (hoa hồng theo doanh thu tối thiểu chỉ có 1 phần tử tham số)
            var updateParam = Param.CommissionParams.FirstOrDefault();
            // Tham số cũ
            var oldParam = oldRuleParam.CommissionParams.FirstOrDefault();
            if (updateParam == null || oldParam == null)
            {
                Param.CommissionSalary = Process();
                return;
            }

            updateParam.IsDirty = oldParam.IsDirty.GetValueOrDefault()
                                  || updateParam.CommissionLevel == oldParam.CommissionLevel
                                  && (oldParam.ValueOrigin != oldParam.Value || oldParam.ValueRatioOrigin != oldParam.ValueRatio);

            if (updateParam.IsDirty.GetValueOrDefault())
            {
                updateParam.Value = oldParam.Value;
                updateParam.ValueRatio = oldParam.ValueRatio;
                Param.CommissionSalary = Process();
            }
        }

        private void UpdateParamsWithPersonalGrossProfit(CommissionSalaryRuleParamV2 oldRuleParam)
        {
            var updateParams = Param.CommissionParams;
            var oldParams = oldRuleParam.CommissionParams;

            updateParams.ForEach(updateParam =>
            {
                var existOldParam = oldParams.FirstOrDefault(x => x.CommissionLevel == updateParam.CommissionLevel);
                updateParam.IsDirty = existOldParam != null &&
                        (existOldParam.IsDirty.GetValueOrDefault() || existOldParam.ValueOrigin != existOldParam.Value || existOldParam.ValueRatioOrigin != existOldParam.ValueRatio);
                if (existOldParam == null || !updateParam.IsDirty.GetValueOrDefault()) return;
                updateParam.Value = existOldParam.Value;
                updateParam.ValueRatio = existOldParam.ValueRatio;
            });


            Param.CommissionSalary = Process();
        }

        private decimal InitTotalRevenueCommission(EngineResource resource)
        {
            if (Value.FormalityTypes == CommissionSalaryFormalityTypes.PersonalCommissionRevenue)
                return resource.TotalRevenue;

            if (Value.FormalityTypes == CommissionSalaryFormalityTypes.BranchCommissionRevenue)
            {
                if (resource.BranchProductRevenues == null || resource.BranchProductRevenues.Count == 0) return 0;

                if (Value.IsAllBranch) return resource.BranchProductRevenues.Sum(b => b.TotalCommission);

                if (Value.BranchIds == null || Value.BranchIds.Count == 0) return 0;

                return resource.BranchProductRevenues.Where(b => Value.BranchIds.Contains(b.BranchId))
                    .Sum(b => b.TotalCommission);
            }

            return 0;
        }

        //Lấy tổng lợi nhuận gộp của nhân viên
        private decimal InitTotalGrossProfitCommission(EngineResource resource)
        {
            if (Value.FormalityTypes == CommissionSalaryFormalityTypes.PersonalGrossProfit)
            {
                return resource.TotalGrossProfit;
            }

            return 0;
        }

        private List<ProductRevenue> InitProductRevenuesCommission(EngineResource resource, byte commissionType)
        {
            var productRevenuesGroup = new List<ProductRevenue>();

            if (Value.FormalityTypes == CommissionSalaryFormalityTypes.PersonalCommissionRevenue)
            {
                if (commissionType == (byte)CommissionType.Service)
                {
                    return resource.ProductRevenues;
                }
                else
                {
                    return resource.ProductCounselorRevenues;
                }
            }

            if (Value.FormalityTypes == CommissionSalaryFormalityTypes.BranchCommissionRevenue && commissionType == (byte)CommissionType.Service)
            {
                if (resource.BranchProductRevenues == null || resource.BranchProductRevenues.Count == 0)
                    return new List<ProductRevenue>();

                productRevenuesGroup = resource.BranchProductRevenues;
                if (!Value.IsAllBranch)
                {
                    if (Value.BranchIds == null || Value.BranchIds.Count == 0) return new List<ProductRevenue>();
                    productRevenuesGroup = productRevenuesGroup.Where(b => Value.BranchIds.Contains(b.BranchId)).ToList();
                }

                productRevenuesGroup = productRevenuesGroup.GroupBy(b => b.ProductId).Select(b =>
                    new ProductRevenue
                    {
                        ProductId = b.Key,
                        Quantity = b.Sum(q => q.Quantity),
                        TotalCommission = b.Sum(q => q.TotalCommission),
                        CategoryIds = b.FirstOrDefault()?.CategoryIds
                    }).ToList();
            }

            return productRevenuesGroup;
        }

        private long GetCommissionDetailCategoryId(CommissionParam ruleParamDetail, ProductRevenue productRevenue)
        {
            var categoryIdsInCommissionDetail = ruleParamDetail.CommissionTable
                                                                .CommissionDetails
                                                                .Where(c => c.Type == (byte)ObjectType.Category)
                                                                .Select(c => c.ObjectId)
                                                                .ToList();
            var productCategoryIds = productRevenue.CategoryIds.Split(',').Select(long.Parse).ToList();
            var applyCategoryId = productCategoryIds
                                .FirstOrDefault(c => categoryIdsInCommissionDetail.Contains(c));
            return applyCategoryId;
        }
        #endregion
    }
}
