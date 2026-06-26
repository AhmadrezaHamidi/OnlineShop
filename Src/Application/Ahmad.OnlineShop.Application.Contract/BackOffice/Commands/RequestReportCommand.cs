using AhmadBase.Application;
using BackOffice.Domain.Enums;

namespace BackOffice.Application.Commands;

public record RequestReportCommand(
    long       AdminId,
    long       ReportId,
    ReportType Type
) : ICommand<long>;
