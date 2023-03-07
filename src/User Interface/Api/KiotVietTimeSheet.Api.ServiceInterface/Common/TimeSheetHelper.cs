using System.Collections.Generic;
using System.Threading.Tasks;
using KiotVietTimeSheet.Application.Runtime.Exception;
using KiotVietTimeSheet.Infrastructure.DbMaster;
using KiotVietTimeSheet.Resources;
using KiotVietTimeSheet.Utilities;
using Newtonsoft.Json;
using ServiceStack;
using ServiceStack.Configuration;

namespace KiotVietTimeSheet.Api.ServiceInterface.Common
{
    public class TimeSheetHelper
    {
        private readonly IMasterDbService _masterDbService;
        public TimeSheetHelper(IMasterDbService masterDbService)
        {
            _masterDbService = masterDbService;
        }

        public async Task<string> GetRetailerCodeAsync(int retailerId)
        {
            var retailer = await _masterDbService.GetRetailerAsync(retailerId);
            if (retailer == null)
            {
                throw new KvTimeSheetException(Message.error_whenUpdateData);
            }
            return retailer.Code;
        }
        public async Task<(int, string)> GetBlockUnitByRetailerId(int retailerId)
        {
            var retailer = await _masterDbService.GetRetailerAsync(retailerId);
            if (retailer == null)
            {
                throw new KvTimeSheetException(Message.error_whenUpdateData);
            }

            var blockUnit = 0;
            if (retailer.TimeSheetBlockUnit == null)
            {
                var tsBlockUnitByContract =
                    JsonConvert.DeserializeObject<Dictionary<string, int>>(HostContext.Resolve<IAppSettings>()
                        .GetString("TimeSheetUnitByContract"));
                if (tsBlockUnitByContract == null)
                {
                    throw new KvTimeSheetException(Message.error_whenUpdateData);
                }

                switch (retailer.ContractType.GetValueOrDefault(0))
                {
                    case (byte)ContractTypes.Trial:
                        blockUnit = tsBlockUnitByContract[nameof(ContractTypes.Trial)];
                        break;
                    case (byte)ContractTypes.Basic:
                        blockUnit = tsBlockUnitByContract[nameof(ContractTypes.Basic)];
                        break;
                    case (byte)ContractTypes.Advance:
                        blockUnit = tsBlockUnitByContract[nameof(ContractTypes.Advance)];
                        break;
                }

                return (blockUnit, retailer.Code);
            }

            return (retailer.TimeSheetBlockUnit.GetValueOrDefault(), retailer.Code);

        }

        public async Task<int> GetContractTypeByRetailerId(int retailerId)
        {
            var retailer = await _masterDbService.GetRetailerAsync(retailerId);
            if (retailer == null)
            {
                throw new KvTimeSheetException(Message.error_whenUpdateData);
            }

            var contractType = retailer.ContractType ?? (int)ContractTypes.Basic;

            return contractType;

        }
    }
}