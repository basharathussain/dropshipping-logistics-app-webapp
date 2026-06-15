using Logistics.Domain.Entities;
using Logistics.Domain.Primitives.Enums;
using Logistics.Domain.Specifications;

namespace Logistics.Application.Modules.Platform.BlogPosts.Specifications;

public sealed class GetPublishedBlogPosts : BaseSpecification<BlogPost>
{
    public GetPublishedBlogPosts(
        string? orderBy,
        int page,
        int pageSize,
        string? search = null,
        string? category = null)
    {
        var searchLower = search?.ToLower() ?? string.Empty;
        Criteria = x =>
            x.Status == BlogPostStatus.Published &&
            (string.IsNullOrEmpty(searchLower) ||
             x.Title.ToLower().Contains(searchLower) ||
             (x.Excerpt != null && x.Excerpt.ToLower().Contains(searchLower))) &&
            (string.IsNullOrEmpty(category) || x.Category == category);

        OrderBy(string.IsNullOrEmpty(orderBy) ? "-PublishedAt" : orderBy);
        ApplyPaging(page, pageSize);
    }
}
