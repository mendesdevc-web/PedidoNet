using PedidosNet.Models;

namespace PedidosNet.Domain
{
    public class PedidoFactory : IPedidoFactory
    {
        public Pedido Criar(Cliente cliente, IReadOnlyCollection<ItemPedido> itens)
        {
            var pedido = new Pedido
            {
                Id = Guid.NewGuid(),
                ClienteId = cliente.Id,
                DataCriacao = DateTime.Now,
                //Pedido.Itens continua List<ItemPedido> internamente, - a defesa
                //é so na fronteira do contrato: quem chama Criar nao pode mais
                //segurar a mesma lista e mutá-la depois de o Pedido já existir
                Itens = itens.ToList(),
            };

            // O pedido sabe somar seus proprios itens - a Factory so pede.
            pedido.CalcularTotal();

            pedido.Status = DefinirStatusInicial(cliente, pedido.Total);

            return pedido;
        }

        // Prioridade explicita e comparavel - não mais posiçao de linha
        private static string DefinirStatusInicial(Cliente cliente, decimal total)
        {
           if(cliente.Corporativo && total > 5000) return "AguardandoValidacao";

           if (cliente.Vip) return "EmAnalise";
           
           return "Pendente";
        }
    }
}
