using KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Models;
using KiotVietTimeSheet.SharedKernel.Specifications;

namespace KiotVietTimeSheet.Domain.AggregatesModels.GpsInfoAggregate.Specifications
{
    public class FindGpsInfoByQrKeySpec : ExpressionSpecification<GpsInfo>
    {
        public FindGpsInfoByQrKeySpec(string qrKey)
            : base(g => g.QrKey == qrKey) { }
    }
}
