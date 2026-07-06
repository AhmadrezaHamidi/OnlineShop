using AhmadBase.Application;

namespace BackOffice.Application.Commands;

public record ActivateAdminCommand(long AdminId) : ICommand<long>;
