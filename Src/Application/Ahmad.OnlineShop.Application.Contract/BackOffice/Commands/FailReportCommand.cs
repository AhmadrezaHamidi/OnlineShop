using AhmadBase.Application;
using System.Text.Json.Serialization;

namespace BackOffice.Application.Commands;

public record FailReportCommand(
    long   AdminId,
    [property: JsonIgnore] long ReportId,
    string Reason
) : ICommand<long>;
