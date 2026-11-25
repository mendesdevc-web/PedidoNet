using PedidosNet.Domain.Entities;

namespace PedidosNet.Domain.Validacoes
{
    public abstract class PedidoValidationHandlerBase : IPedidoValidationHandler
    {
        private IPedidoValidationHandler? _next;

        public IPedidoValidationHandler SetNext(IPedidoValidationHandler next)
        {
            _next = next;
            return next;
        }

        public async Task<Result> HandleAsync(
            Cliente cliente,
            IReadOnlyCollection<ItemPedido> itens)
        {
            var result = await ValidarAsync(cliente, itens);

            if (!result.IsSuccess)
                return result;

            if (_next is not null)
                return await _next.HandleAsync(cliente, itens);

            return Result.Success();
        }

        protected abstract Task<Result> ValidarAsync(Cliente cliente,IReadOnlyCollection<ItemPedido> itens);
    }
}
