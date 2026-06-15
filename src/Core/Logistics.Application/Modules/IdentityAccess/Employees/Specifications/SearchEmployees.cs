using Logistics.Domain.Entities;
using Logistics.Domain.Specifications;

namespace Logistics.Application.Modules.IdentityAccess.Employees.Specifications;

public class SearchEmployees : BaseSpecification<Employee>
{
    public SearchEmployees(
        string? search,
        string? roleName,
        string? orderBy,
        int page,
        int pageSize)
    {
        if (!string.IsNullOrEmpty(search))
        {
            var searchLower = search.ToLower();
            Criteria = i =>
                (i.FirstName != null && i.FirstName.ToLower().Contains(searchLower)) ||
                (i.LastName != null && i.LastName.ToLower().Contains(searchLower)) ||
                (i.PhoneNumber != null && i.PhoneNumber.ToLower().Contains(searchLower)) ||
                (i.Email != null && i.Email.ToLower().Contains(searchLower));
        }

        if (!string.IsNullOrEmpty(roleName))
        {
            var roleLower = roleName.ToLower();
            AddInclude(e => e.Role!);
            Criteria = Criteria is null
                ? e => e.Role != null && e.Role.Name.ToLower().Contains(roleLower)
                : Criteria.AndAlso(e => e.Role != null && e.Role.Name.ToLower().Contains(roleLower));
        }


        OrderBy(orderBy);
        ApplyPaging(page, pageSize);
    }

    // protected override Expression<Func<Employee, object?>> CreateOrderByExpression(string propertyName)
    // {
    //     return propertyName switch
    //     {
    //         "firstname" => i => i.FirstName,
    //         "lastname" => i => i.LastName,
    //         "phonenumber" => i => i.PhoneNumber,
    //         "salary" => i => i.Salary,
    //         "salarytype" => i => i.SalaryType,
    //         _ => i => i.Email
    //     };
    // }
}
