using Logistics.Domain.Entities;
using Logistics.Domain.Specifications;

namespace Logistics.Application.Modules.IdentityAccess.Roles.Specifications;

public class SearchAppRoles : BaseSpecification<AppRole>
{
    public SearchAppRoles(
        string? search,
        int page,
        int pageSize)
    {
        if (!string.IsNullOrEmpty(search))
        {
            var searchLower = search.ToLower();
            Criteria = i =>
                (i.Name != null && i.Name.ToLower().Contains(searchLower)) ||
                (i.DisplayName != null && i.DisplayName.ToLower().Contains(searchLower));
        }

        ApplyPaging(page, pageSize);
    }
}
