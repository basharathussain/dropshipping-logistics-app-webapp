using Logistics.Domain.Entities;
using Logistics.Domain.Specifications;

namespace Logistics.Application.Modules.IdentityAccess.Users.Specifications;

public class SearchUsers : BaseSpecification<User>
{
    public SearchUsers(
        string? search,
        string? orderBy,
        int page,
        int pageSize)
    {
        if (!string.IsNullOrEmpty(search))
        {
            var searchLower = search.ToLower();
            Criteria = i =>
                i.FirstName.ToLower().Contains(searchLower) ||
                i.LastName.ToLower().Contains(searchLower) ||
                (i.PhoneNumber != null && i.PhoneNumber.ToLower().Contains(searchLower)) ||
                (i.Email != null && i.Email.ToLower().Contains(searchLower));
        }

        OrderBy(orderBy);
        ApplyPaging(page, pageSize);
    }

    // protected override Expression<Func<User, object?>> CreateOrderByExpression(string propertyName)
    // {
    //     return propertyName switch
    //     {
    //         "firstname" => i => i.FirstName,
    //         "lastname" => i => i.LastName,
    //         "phonenumber" => i => i.PhoneNumber,
    //         _ => i => i.Email
    //     };
    // }
}
