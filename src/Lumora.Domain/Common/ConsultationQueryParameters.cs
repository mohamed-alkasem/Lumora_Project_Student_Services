using Lumora.Domain.Enums;

namespace Lumora.Domain.Common;

public class ConsultationQueryParameters
{
    public const int MaxPageSize = 100;

    public ConsultationStatus? Status { get; set; }

    private int _page = 1;
    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    private int _pageSize = 20;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1 ? 20 : Math.Min(value, MaxPageSize);
    }
}
