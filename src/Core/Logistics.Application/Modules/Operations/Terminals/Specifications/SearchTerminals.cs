using Logistics.Domain.Entities;
using Logistics.Domain.Primitives.Enums;
using Logistics.Domain.Specifications;

namespace Logistics.Application.Modules.Operations.Terminals.Specifications;

public class SearchTerminals : BaseSpecification<Terminal>
{
    public SearchTerminals(
        string? search,
        string? orderBy,
        int page,
        int pageSize,
        TerminalType? type = null,
        string? countryCode = null)
    {
        var searchLower = search?.ToLower() ?? string.Empty;
        Criteria = i =>
            (string.IsNullOrEmpty(searchLower) || i.Name.ToLower().Contains(searchLower) || i.Code.ToLower().Contains(searchLower)) &&
            (!type.HasValue || i.Type == type.Value) &&
            (string.IsNullOrEmpty(countryCode) || i.CountryCode == countryCode);

        OrderBy(orderBy);
        ApplyPaging(page, pageSize);
    }
}
