using AhmadBase.Application;
using System.Text.Json.Serialization;

namespace BackOffice.Application.Commands;

public record CompleteReportCommand(
    long   AdminId,
    [property: JsonIgnore] long ReportId,
    string FilePath
) : ICommand<long>;
