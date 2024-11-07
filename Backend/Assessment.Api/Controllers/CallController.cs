﻿using Assessment.Application.Calls.Commands;
using Assessment.Application.Calls.Queries;

namespace Assessment.Api.Controllers;

public class CallController : BaseApiController
{
    private readonly IMediator _mediator;

    public CallController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<PaginatedResult<GetPaginatedCallsQueryRow>> GetPaginatedCallsAsync(
        [FromQuery] GetPaginatedCallsQuery query) =>
        await _mediator.Send(query);
    
    [HttpPost]
    public async Task<Result<CreateCallResult>> CreateCallAsync(
        [FromBody] CreateCallCommand command) =>
        await _mediator.Send(command);

    [HttpGet("dailystats")]
    public async Task<PaginatedResult<GetCallsPerHourResultRow>> GetCallsPerHour(
        [FromQuery] GetCallsPerHourQuery query) =>
        await _mediator.Send(query);

    [HttpGet("summarystats")]
    public async Task<Result<SummaryStats>> GetSummaryStats(
        [FromQuery] GetSummaryStatsQuery query) =>
        await _mediator.Send(query);
}