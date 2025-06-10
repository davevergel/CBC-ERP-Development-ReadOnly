using CbcRoastersErp.Models.Finance;
using CbcRoastersErp.Models;

namespace CbcRoastersErp.Services.Finance
{
    public static class BigCommerceMetricsMapper
    {
        public static List<DriposSalesMetric> MapToSalesMetrics(IEnumerable<BigCommerceOrders> orders)
        {
            return orders
                .Where(o => o.TotalAmount > 0 && o.OrderDate != default)
                .Select(o => new DriposSalesMetric
                {
                    MetricDate = (DateTime)o.OrderDate,
                    MetricName = "BigCommerce Order",
                    Amount = o.TotalAmount,
                    Source = "BigCommerce",
                    CreatedAt = DateTime.Now
                }).ToList();
        }
    }
}
