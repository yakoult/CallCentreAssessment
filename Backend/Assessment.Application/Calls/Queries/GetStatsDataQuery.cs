using Assessment.Data;
using Assessment.Data.Entities;
using Assessment.Shared.Extensions;
using Assessment.Shared.Models;
using MediatR;

namespace Assessment.Application.Calls.Queries;

public class GetSummaryStatsQuery : IRequest<Result<SummaryStats>> { }

public class SummaryStats
{
    public DateTime DateWithMostCalls { get; set; }
    public double AvgCallsPerDay { get; set; }
    public double AvgCallsPerUser { get; set; }
}
public class GetSummaryStatsQueryHandler : IRequestHandler<GetSummaryStatsQuery, Result<SummaryStats>>
{
    private readonly ApplicationDbContext _context;

    public GetSummaryStatsQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<SummaryStats>> Handle(
        GetSummaryStatsQuery request,
        CancellationToken cancellationToken)
    {
        var userQuery = _context.Calls
        .GroupBy(c => c.CallingUserId)
        .Select(g => new
        {
            UserId = g.Key,
            CallCount = g.Count(),
        })
        .ToList();

        var dateQuery = _context.Calls
        .GroupBy(c => c.DateCallStarted.Date)
        .Select(g => new
        {
            Date = g.Key,
            CallCount = g.Count()
        })
        .OrderByDescending(g => g.CallCount);

        DateTime dateWithMostCalls = dateQuery.FirstOrDefault().Date;

        var avgCallsPerCaller = userQuery
            .Average(g => g.CallCount);

        var avgCallsPerDay = dateQuery.Average(g => g.CallCount);
        var result = new SummaryStats
        {
            DateWithMostCalls = dateWithMostCalls,
            AvgCallsPerDay = avgCallsPerDay,
            AvgCallsPerUser = avgCallsPerCaller
        };

        return Result<SummaryStats>.Success(result);
    }
}