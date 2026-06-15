using Logistics.Domain.Entities;
using Logistics.Domain.Specifications;

namespace Logistics.Application.Modules.Platform.Contacts.Specifications;

public sealed class GetContactSubmissions : BaseSpecification<ContactSubmission>
{
    public GetContactSubmissions(string? orderBy, int page, int pageSize, string? search = null)
    {
        if (!string.IsNullOrEmpty(search))
        {
            var searchLower = search.ToLower();
            Criteria = x =>
                x.Email.ToLower().Contains(searchLower) ||
                x.FirstName.ToLower().Contains(searchLower) ||
                x.LastName.ToLower().Contains(searchLower) ||
                x.Message.ToLower().Contains(searchLower);
        }

        OrderBy(orderBy);
        ApplyPaging(page, pageSize);
    }
}
