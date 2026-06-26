using AhmadBase.Application;

namespace BackOffice.Application.Commands;

public record DeactivateAdminCommand(long AdminId) : ICommand<long>;
