using Logistics.Domain.Entities;
using Logistics.Domain.Specifications;

namespace Logistics.Application.Modules.IdentityAccess.Tenants.Specifications;

public class SearchTenants : BaseSpecification<Tenant>
{
    public SearchTenants(
        string? search,
        string? orderBy,
        int page,
        int pageSize)
    {
        if (!string.IsNullOrEmpty(search))
        {
            var searchLower = search.ToLower();
            Criteria = i =>
                (!string.IsNullOrEmpty(i.Name) &&
                 i.Name.ToLower().Contains(searchLower)) ||

                (!string.IsNullOrEmpty(i.CompanyName) &&
                 i.CompanyName.ToLower().Contains(searchLower));
        }

        OrderBy(orderBy);
        ApplyPaging(page, pageSize);
    }

    // protected override Expression<Func<Tenant, object?>> CreateOrderByExpression(string propertyName)
    // {
    //     return propertyName switch
    //     {
    //         "companyname" => i => i.CompanyName,
    //         "companyaddress" => i => i.CompanyAddress.Line1,
    //         "subscriptionplan" => i => i.Subscription!.Plan.Name,
    //         _ => i => i.Name
    //     };
    // }
}
