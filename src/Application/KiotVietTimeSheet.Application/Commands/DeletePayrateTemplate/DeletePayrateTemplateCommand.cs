using KiotVietTimeSheet.Application.Abstractions;
using KiotVietTimeSheet.Application.Auth;
using MediatR;

namespace KiotVietTimeSheet.Application.Commands.DeletePayrateTemplate
{
    [Auth.Common.RequiredPermission(TimeSheetPermission.GeneralSettingPayrateTemplate_Delete)]
    public class DeletePayrateTemplateCommand : BaseCommand<Unit>
    {
        public long Id { get; set; }
        public bool IsGeneralSetting { get; set; }

        public DeletePayrateTemplateCommand(long id, bool isGeneralSetting)
        {
            Id = id;
            IsGeneralSetting = isGeneralSetting;
        }
    }
}
