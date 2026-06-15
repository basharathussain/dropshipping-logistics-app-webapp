using Logistics.Domain.Entities;
using Logistics.Domain.Primitives.Enums;
using Logistics.Domain.Specifications;

namespace Logistics.Application.Modules.Operations.Containers.Specifications;

public class SearchContainers : BaseSpecification<Container>
{
    public SearchContainers(
        string? search,
        string? orderBy,
        int page,
        int pageSize,
        ContainerStatus? status = null,
        ContainerIsoType? isoType = null,
        Guid? currentTerminalId = null)
    {
        var searchLower = search?.ToLower() ?? string.Empty;
        Criteria = i =>
            (string.IsNullOrEmpty(searchLower) || i.Number.ToLower().Contains(searchLower)
                                          || (i.BookingReference != null && i.BookingReference.ToLower().Contains(searchLower))
                                          || (i.BillOfLadingNumber != null && i.BillOfLadingNumber.ToLower().Contains(searchLower))) &&
            (!status.HasValue || i.Status == status.Value) &&
            (!isoType.HasValue || i.IsoType == isoType.Value) &&
            (!currentTerminalId.HasValue || i.CurrentTerminalId == currentTerminalId.Value);

        OrderBy(orderBy);
        ApplyPaging(page, pageSize);
    }
}
