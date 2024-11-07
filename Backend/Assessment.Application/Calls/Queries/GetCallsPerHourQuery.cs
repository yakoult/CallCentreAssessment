using Assessment.Data;
using Assessment.Data.Entities;
using Assessment.Shared.Extensions;
using Assessment.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Assessment.Application.Calls.Queries;
public class GetCallsPerHourQuery : PaginatedRequest<GetCallsPerHourResultRow> { }

public class GetCallsPerHourRow
{
    public int Hour { get; set; }
    public int CallCount { get; set; }
    public string TopUserId { get; set; }
}
public class GetCallsPerHourResultRow
{
    public int Hour { get; set; }
    public int CallCount { get; set; }
    public string TopUser { get; set; }
}

public class GetCallsPerHourQueryHandler
: IRequestHandler<GetCallsPerHourQuery, PaginatedResult<GetCallsPerHourResultRow>>
{
    private readonly ApplicationDbContext _context;

    public GetCallsPerHourQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<GetCallsPerHourResultRow>> Handle(
        GetCallsPerHourQuery request,
        CancellationToken cancellationToken)
    {
        var isDescending = request.SortDirection == SortDirection.Descending;

        if (string.IsNullOrWhiteSpace(request.SortBy))
        {
            isDescending = true;
            request.SortBy = nameof(Call.DateCallStarted);
        }

        var query = _context
            .Calls
            .Include(c => c.CallingUser)
            .Where(x => x.DateCallStarted.Date == DateTime.Now.Date && (x.DateCallStarted.Hour > 9 && x.DateCallStarted.Hour < 17))
            .GroupBy(u => u.DateCallStarted.Hour)
            .Select(g => new
            {
                Hour = g.Key,
                CallCount = g.Count(),
                TopUserId = g
            .GroupBy(c => c.CallingUserId)
            .OrderByDescending(c => c.Count())
            .Select(c => c.Key)
            .FirstOrDefault()
            })
            .AsQueryable()
            .Join(_context.Users,
                callStats => callStats.TopUserId,
                user => user.Id,
                (callStats, user) => new GetCallsPerHourResultRow
                {
                    Hour = callStats.Hour,
                    CallCount = callStats.CallCount,
                    TopUser = user.Username
                })
            .OrderByDescending(g => g.CallCount)
            .ThenBy(g => g.Hour)
            .AsQueryable();

        return await query.PaginateAsync(request);
    }
}
