using PedidosNet.Domain.Entities;

namespace PedidosNet.Domain.Validacoes
{

    public class ClienteBloqueadoHandler : PedidoValidationHandlerBase
    {
        protected override Task<Result> ValidarAsync(Cliente cliente, IReadOnlyCollection<ItemPedido> itens)
        {
            if (cliente.Bloqueado)
            {
                return Task.FromResult(
                    Result.Failure("cliente_bloqueado", "Cliente Bloqueado não pode criar pedidos."));
            }

            return Task.FromResult(Result.Success());
        }
    }
}
