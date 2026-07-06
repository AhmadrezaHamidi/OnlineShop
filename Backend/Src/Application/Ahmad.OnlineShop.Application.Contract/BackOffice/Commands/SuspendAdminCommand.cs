using AhmadBase.Application;

namespace BackOffice.Application.Commands;

public record SuspendAdminCommand(long AdminId) : ICommand<long>;
