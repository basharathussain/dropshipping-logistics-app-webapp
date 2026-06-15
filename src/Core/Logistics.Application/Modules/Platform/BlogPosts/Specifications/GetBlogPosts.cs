using Logistics.Domain.Entities;
using Logistics.Domain.Primitives.Enums;
using Logistics.Domain.Specifications;

namespace Logistics.Application.Modules.Platform.BlogPosts.Specifications;

public sealed class GetBlogPosts : BaseSpecification<BlogPost>
{
    public GetBlogPosts(
        string? orderBy,
        int page,
        int pageSize,
        string? search = null,
        string? category = null,
        BlogPostStatus? status = null)
    {
        var searchLower = search?.ToLower() ?? string.Empty;
        Criteria = x =>
            (string.IsNullOrEmpty(searchLower) ||
             x.Title.ToLower().Contains(searchLower) ||
             x.Content.ToLower().Contains(searchLower) ||
             (x.Excerpt != null && x.Excerpt.ToLower().Contains(searchLower))) &&
            (string.IsNullOrEmpty(category) || x.Category == category) &&
            (!status.HasValue || x.Status == status.Value);

        OrderBy(orderBy);
        ApplyPaging(page, pageSize);
    }
}
