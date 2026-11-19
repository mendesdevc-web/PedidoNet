using PedidosNet.Models;

namespace PedidosNet.Domain.Criacao
{
    public class PedidoFactory : IPedidoFactory
    {
        public Pedido Criar(Cliente cliente, IReadOnlyCollection<ItemPedido> itens)
        {
            var pedido = new Pedido
            {
                Id = Guid.NewGuid(),
                ClienteId = cliente.Id,
                Cliente = cliente,
                DataCriacao = DateTime.Now,
                EnderecoEntrega = cliente.Endereco,
                Itens = itens.ToList(),
            };

            pedido.CalcularTotal();

            pedido.Status = DefinirStatusInicial(cliente, pedido.Total);

            return pedido;
        }

        private static string DefinirStatusInicial(Cliente cliente, decimal total)
        {
           if(cliente.Segmento == SegmentoCliente.Corporativo && total > 5000) return "AguardandoValidacao";

           if (cliente.Segmento == SegmentoCliente.Vip) return "EmAnalise";
           
           return "Pendente";
        }
    }
}
