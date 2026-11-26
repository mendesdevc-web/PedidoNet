using PedidosNet.Application.Commands;
using PedidosNet.Application.Handlers;
using PedidosNet.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace PedidosNet.Application.Dispatchers
{
    public sealed class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
    {
        public async Task<Result> SendAsync(ICommand command)
        {
            var handlerType = typeof(ICommandHandler<>)
                .MakeGenericType(command.GetType());

            var handler = serviceProvider.GetRequiredService(handlerType);

            var method = handlerType.GetMethod(
                nameof(ICommandHandler<ICommand>.HandleAsnync));

            if (method is null)
                throw new InvalidOperationException(
                    $"O Handler para o Command {command.GetType().Name} não possui o método HandleAsnync.");

            return await (Task<Result>)method.Invoke(handler, [command])!;
        }

        public async Task<Result<TResult>> SendAsync<TResult>(
            ICommand<TResult> command)
        {
            var handlerType = typeof(ICommandHandler<,>)
                .MakeGenericType(command.GetType(), typeof(TResult));

            var handler = serviceProvider.GetRequiredService(handlerType);

            var method = handlerType.GetMethod("HandleAsync")
                ?? throw new InvalidOperationException(
                    $"O Handler para o Command {command.GetType().Name} " +
                    "não possui o método HandleAsync.");

            return await (Task<Result<TResult>>)method.Invoke(
                handler, [command])!;
        }
    }
}