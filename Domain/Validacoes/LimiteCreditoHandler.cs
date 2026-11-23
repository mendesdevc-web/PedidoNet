using PedidosNet.Models;

namespace PedidosNet.Domain.Validacoes
{
    public class LimiteCreditoHandler : PedidoValidationHandlerBase
    {
        protected override Task<Result> ValidarAsync(Cliente cliente, IReadOnlyCollection<ItemPedido> itens)
        {
            var total = itens.Sum(i => i.Preco * i.Quantidade);
            if (total > cliente.LimiteCredito)
            {
                return Task.FromResult(Result.Failure("Limite_cretido_insuficinete", "Limite de cretido insuficinete"));
            }
            return Task.FromResult(Result.Success());
        }
    }
}
