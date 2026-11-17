using PedidosNet.Models;

namespace PedidosNet.Domain
{
    public class KitPromocionalBuilder
    {
        private readonly IPedidoFactory _pedidoFactory;

        private Cliente? _cliente;
        private readonly List<ItemPedido> _itens = new();
        private decimal _pesoAcumulado;
        private decimal _valorAcumulado;
        private string? _cupomDesconto;

        private const decimal PesoMaximoKit = 5.0m;
        private const decimal ValorMinimoParaCupom = 300.0m;
        private const int MinimoProdutosDistindo = 2;
        private const decimal PercentualDesconto = 0.10m;

        public KitPromocionalBuilder(IPedidoFactory pedidoFactory)
        {
            _pedidoFactory = pedidoFactory;
        }

        public KitPromocionalBuilder ParaCliente(Cliente cliente)
        {
            _cliente = cliente;
            return this;
        }

        public KitPromocionalBuilder AddProduto(Produto produto,int quantidade)
        {
            var pesoItem = produto.Peso * quantidade;
            var pesoResultante = _pesoAcumulado + pesoItem;

            if(pesoResultante > PesoMaximoKit)
                throw new InvalidOperationException($"O peso total do kit promocional não pode exceder {PesoMaximoKit} kg.");

            _itens.Add(new ItemPedido
            {
                Id = Guid.NewGuid(),
                ProdutoId = produto.Id,
                NomeProduto = produto.Nome,
                Preco = produto.Preco,
                Quantidade = quantidade,
                Peso = produto.Peso
            });

            _pesoAcumulado = pesoResultante;
            _valorAcumulado += produto.Preco * quantidade;

            return this;
        }

        public KitPromocionalBuilder AplicarCupom(string cumpomDesconto)
        {
            if (_valorAcumulado < ValorMinimoParaCupom)
                throw new InvalidOperationException($"O valor total do kit promocional deve ser de pelo menos {ValorMinimoParaCupom:C} para aplicar o cupom de desconto.");
           
            if (_itens.Select(i => i.ProdutoId).Distinct().Count() < MinimoProdutosDistindo)
                throw new InvalidOperationException($"O kit promocional deve conter pelo menos {MinimoProdutosDistindo} produtos distintos para aplicar o cupom de desconto.");
            
            _cupomDesconto = cumpomDesconto;
            return this;
        }

        public Pedido Build()
        {
            if (_cliente == null)
                throw new InvalidOperationException("O cliente deve ser definido antes de construir o pedido.");

            if(_itens.Select(i => i.ProdutoId).Distinct().Count() < MinimoProdutosDistindo)
                throw new InvalidOperationException($"O kit promocional deve conter pelo menos {MinimoProdutosDistindo} produtos distintos.");

           
            var pedido = _pedidoFactory.Criar(_cliente, _itens);

            if(_cupomDesconto is not null)
                pedido.AplicarCupom(_cupomDesconto, PercentualDesconto);

            return pedido;

        }
    }
}
