using AhmadBase.Application;

namespace BackOffice.Application.Commands;

public record FailReportCommand(
    long   AdminId,
    long   ReportId,
    string Reason
) : ICommand<long>;
