using PedidosNet.Domain.Estado;
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

            var estadoInical = DefinirStatusInicial(cliente, pedido.Total);
            pedido.AlterarEstado(estadoInical);
            
            return pedido;
        }

        private static  IPedidoState DefinirStatusInicial(Cliente cliente, decimal total)
        {
           if(cliente.Segmento == SegmentoCliente.Corporativo && total > 5000)
           {
                return new PedidoAguardandoValidacaoState();
           }

           if (cliente.Segmento == SegmentoCliente.Vip)
           {
                return new PedidoEmAnaliseState();
           }
           
           return new PedidoPendenteState();
        }
    }
}
