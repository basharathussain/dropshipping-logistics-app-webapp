using Logistics.Domain.Entities;
using Logistics.Domain.Specifications;
using System.Linq.Expressions;

namespace Logistics.Application.Modules.Platform.DemoRequests.Specifications;

public sealed class GetDemoRequests : BaseSpecification<DemoRequest>
{
    public GetDemoRequests(string? orderBy, int page, int pageSize, string? search = null)
    {
        if (!string.IsNullOrEmpty(search))
        {
            var searchLower = search.ToLower();
            Criteria = x =>
                x.Email.ToLower().Contains(searchLower) ||
                x.FirstName.ToLower().Contains(searchLower) ||
                x.LastName.ToLower().Contains(searchLower) ||
                x.Company.ToLower().Contains(searchLower);
        }

        OrderBy(orderBy);
        ApplyPaging(page, pageSize);
    }
}
