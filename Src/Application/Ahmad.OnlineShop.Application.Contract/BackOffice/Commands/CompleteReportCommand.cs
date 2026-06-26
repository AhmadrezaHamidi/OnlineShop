using AhmadBase.Application;

namespace BackOffice.Application.Commands;

public record CompleteReportCommand(
    long   AdminId,
    long   ReportId,
    string FilePath
) : ICommand<long>;
