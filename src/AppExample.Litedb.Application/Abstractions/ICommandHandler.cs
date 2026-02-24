namespace AppExample.Litedb.Application.Abstractions;

public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    void Handle(TCommand command);
}
