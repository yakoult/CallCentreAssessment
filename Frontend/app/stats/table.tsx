"use client";

import { DefaultPagination, usePaginatedQuery, useApiQuery } from "@/hooks/query";
import { useEffect, useState } from "react";
import { Table } from "../../components/Table";
import Pagination from "../../components/Pagination";
import { Card } from "@/components/Card";

interface Stats {
  hour: number;
  callCount: number;
  topUser: string;
}

interface SummaryStats {
    dateWithMostCalls: string,
    avgCallsPerDay: number,
    avgCallsPerUser: number
}

function convertTo12HourFormat(hour: number): string {
    let period = hour >= 12 ? 'PM' : 'AM';
    let hourIn12 = hour % 12; // Convert hour to 12-hour format
    if (hourIn12 === 0) {
        hourIn12 = 12; // Midnight or noon should show 12
    }
    return `${hourIn12} ${period}`;
}

const StatsTable = () => {
  const [page, setPage] = useState(DefaultPagination.page);

  const { data, isLoading } = usePaginatedQuery<Stats>(
    "/call/dailystats",
    { page },
    { staleTime: 0 }
  );

  const statData = useApiQuery<SummaryStats>("/call/summarystats").data;

  const onNext = () => {
    if (page === data?.totalPages) return;

    setPage(page + 1);
  };

  const onPrevious = () => {
    if (page === 1) return;

    setPage(page - 1);
  };

  return (
    <div>
      <div className="flex mb-4 justify-between">
        <Card>
          <Card.Title>
            {statData != null ? new Date(statData.dateWithMostCalls).toDateString() : ""}
            </Card.Title>
          <Card.Body>Most Calls Received</Card.Body>
        </Card>
        <Card>
          <Card.Title>{statData != null ? Math.floor(statData?.avgCallsPerDay * 100) / 100 : "0"}</Card.Title>
          <Card.Body>Avg Calls Per Day</Card.Body>
        </Card>
        <Card>
          <Card.Title>{statData != null ? Math.floor(statData?.avgCallsPerUser * 100) / 100 : "0"}</Card.Title>
          <Card.Body>Avg Calls Per User</Card.Body>
        </Card>
      </div>
      <Table.Container>
        {isLoading ? (
          <Table.Loading />
        ) : (
          <Table>
            <Table.Header>
              <Table.HeaderCell>Hour</Table.HeaderCell>
              <Table.HeaderCell>Call Count</Table.HeaderCell>
              <Table.HeaderCell>Top User</Table.HeaderCell>
            </Table.Header>
            <Table.Body>
              {data?.data.map((call) => (
                <Table.Row key={call.hour}>
                  <Table.Cell>{convertTo12HourFormat(call.hour)}</Table.Cell>
                  <Table.Cell>
                    {call.callCount}
                  </Table.Cell>
                  <Table.Cell>
                    {call.topUser}
                  </Table.Cell>
                </Table.Row>
              ))}
            </Table.Body>
          </Table>
        )}
      </Table.Container>
      <Pagination
        page={page}
        totalPages={data?.totalPages}
        onNext={onNext}
        onPrevious={onPrevious}
      />
    </div>
  );
};

export default StatsTable;
