using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using KiotVietTimeSheet.Application.Dto;

namespace KiotVietTimeSheet.Application.Commands.CopyPayrateTemplate
{
    [Auth.Common.RequiredPermission(TimeSheetPermission.GeneralSettingPayrateTemplate_Create)]
    public class CopyPayrateTemplateCommand : BaseCommand<PayRateFormDto>
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public CopyPayrateTemplateCommand(long id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
