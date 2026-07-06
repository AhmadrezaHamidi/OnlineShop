using Ahmad.OnlineShop.Domain.BackOffice.Enums;
using AhmadBase.Application;

namespace BackOffice.Application.Commands;

public record RequestReportCommand(
    long       AdminId,
    long       ReportId,
    ReportType Type
) : ICommand<long>;
