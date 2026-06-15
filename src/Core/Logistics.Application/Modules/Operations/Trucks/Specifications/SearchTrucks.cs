using Logistics.Domain.Entities;
using Logistics.Domain.Specifications;

namespace Logistics.Application.Modules.Operations.Trucks.Specifications;

public sealed class SearchTrucks : BaseSpecification<Truck>
{
    public SearchTrucks(string? search, string? orderBy)
    {
        if (!string.IsNullOrEmpty(search))
        {
            var searchLower = search.ToLower();
            Criteria = i => i.Number.ToLower().Contains(searchLower) ||
                            i.MainDriver != null && (i.MainDriver.FirstName.ToLower().Contains(searchLower) ||
                                                     i.MainDriver.LastName.ToLower().Contains(searchLower)) ||
                            (i.SecondaryDriver != null &&
                             (i.SecondaryDriver.FirstName.ToLower().Contains(searchLower) ||
                              i.SecondaryDriver.LastName.ToLower().Contains(searchLower)));
        }

        OrderBy(orderBy);
    }
}
