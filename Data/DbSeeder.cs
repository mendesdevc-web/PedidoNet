
using PedidosNet.Domain.Estado;
using PedidosNet.Models;

namespace PedidosNet.Data;

// GUIDs fixos de propósito: em aula, você sempre referencia o "Cliente Ana"
// ou o "Pedido do Bruno" pelo mesmo Id, gravação após gravação.
public static class DbSeeder
{
    public static readonly Guid ClienteAnaId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid ClienteBrunoId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid ClienteCarlaId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    public static readonly Guid ClienteDaniloId = Guid.Parse("44444444-4444-4444-4444-444444444444");

    public static readonly Guid ProdutoNotebookId = Guid.Parse("aaaaaaaa-0001-0001-0001-000000000001");
    public static readonly Guid ProdutoMouseId = Guid.Parse("aaaaaaaa-0002-0002-0002-000000000002");
    public static readonly Guid ProdutoTecladoId = Guid.Parse("aaaaaaaa-0003-0003-0003-000000000003");
    public static readonly Guid ProdutoMonitorId = Guid.Parse("aaaaaaaa-0004-0004-0004-000000000004");
    public static readonly Guid ProdutoWebcamId = Guid.Parse("aaaaaaaa-0005-0005-0005-000000000005");
    public static readonly Guid ProdutoHeadsetId = Guid.Parse("aaaaaaaa-0006-0006-0006-000000000006");
    public static readonly Guid ProdutoMicrofonetId = Guid.Parse("aaaaaaaa-0007-0007-0007-000000000007");

    public static readonly Guid PedidoAnaId = Guid.Parse("99999999-0001-0001-0001-000000000001");
    public static readonly Guid PedidoBrunoId = Guid.Parse("99999999-0002-0002-0002-000000000002");
    public static readonly Guid PedidoCarlaId = Guid.Parse("99999999-0003-0003-0003-000000000003");

    public static void Seed(AppDbContext db)
    {
        if (db.Clientes.Any())
            return;

        // CLIENTES

        // CLIENTES
        var clientes = new List<Cliente>
        {
            new(
                ClienteAnaId,
                "Ana Ferreira",
                "ana.ferreira@pedidosnet.com.br",
                new Endereco("15098-045", "Av. Americana", "458", "Centro", "São Paulo", "SP"),
                SegmentoCliente.Vip, 5000m,
                new DateTime(2024, 06, 22)),

            new(
                ClienteBrunoId,
                "Bruno Martins",
                "bruno.martins@pedidosnet.com.br",
                new Endereco("15000-100", "Rua das Flores", "120", "Centro", "São José do Rio Preto", "SP"),
                SegmentoCliente.Normal, 12000,
                new DateTime(2024, 08, 15)),

            new(
                ClienteCarlaId,
                "Carla Souza",
                "carla.souza@pedidosnet.com.br",
                new Endereco("15045-200", "Av. Brasil", "850", "Jardim América", "São José do Rio Preto", "SP"),
                SegmentoCliente.Corporativo, 3000m,
                new DateTime(2024, 09, 10)),

            new(
                ClienteDaniloId,
                "Danilo Ribeiro",
                "danilo.ribeiro@pedidosnet.com.br",
                new Endereco("15025-300", "Rua São Paulo", "320", "Vila Nova", "São José do Rio Preto", "SP"),
                SegmentoCliente.Normal, 2000m,
                new DateTime(2025, 01, 20))
        };


        db.Clientes.AddRange(clientes);

        // RECUPERAÇÃO DOS CLIENTES
        var clienteAna = clientes.First(c => c.Id == ClienteAnaId);
        var clienteBruno = clientes.First(c => c.Id == ClienteBrunoId);
        var clienteCarla = clientes.First(c => c.Id == ClienteCarlaId);

        // PRODUTOS
        var produtos = new List<Produto>
        {
            new() { Id = ProdutoNotebookId, Nome = "Notebook Pro 15\"", Categoria = "Informática", Preco = 6500.00m, Estoque = 12, Peso = 2.5m },
            new() { Id = ProdutoMouseId, Nome = "Mouse Sem Fio", Categoria = "Periféricos", Preco = 90.00m, Estoque = 150, Peso = 0.2m },
            new() { Id = ProdutoTecladoId, Nome = "Teclado Mecânico", Categoria = "Periféricos", Preco = 350.00m, Estoque = 80, Peso = 1.0m },
            new() { Id = ProdutoMonitorId, Nome = "Monitor 27\" 144Hz", Categoria = "Informática", Preco = 1900.00m, Estoque = 25, Peso = 5.0m },
            new() { Id = ProdutoWebcamId, Nome = "Webcam Full HD", Categoria = "Periféricos", Preco = 260.00m, Estoque = 60, Peso = 0.5m },
            new() { Id = ProdutoHeadsetId, Nome = "Headset Gamer", Categoria = "Áudio", Preco = 430.00m, Estoque = 40, Peso = 0.8m },
        };

        db.Produtos.AddRange(produtos);

        // PEDIDO ANA
        var pedidoAna = new Pedido
        {
            Id = PedidoAnaId,
            ClienteId = clienteAna.Id,
            EnderecoEntrega = clienteAna.Endereco,
            DataCriacao = new DateTime(2025, 5, 12),
            Itens = new List<ItemPedido>
            {
                new()
                {
                    Id = Guid.Parse("bbbbbbbb-0001-0001-0001-000000000001"),
                    ProdutoId = ProdutoNotebookId,
                    NomeProduto = "Notebook Pro 15\"",
                    Preco = 6500.00m,
                    Quantidade = 1
                },
                new()
                {
                    Id = Guid.Parse("bbbbbbbb-0001-0001-0001-000000000002"),
                    ProdutoId = ProdutoMouseId,
                    NomeProduto = "Mouse Sem Fio",
                    Preco = 90.00m,
                    Quantidade = 1
                },
            }
        };

        pedidoAna.CalcularTotal();

        pedidoAna.AlterarEstado(new PedidoPendenteState());

        // PEDIDO BRUNO
        var pedidoBruno = new Pedido
        {
            Id = PedidoBrunoId,
            ClienteId = clienteBruno.Id,
            EnderecoEntrega = clienteBruno.Endereco,
            DataCriacao = new DateTime(2025, 6, 2),
            Itens = new List<ItemPedido>
            {
                new()
                {
                    Id = Guid.Parse("bbbbbbbb-0002-0002-0002-000000000001"),
                    ProdutoId = ProdutoMonitorId,
                    NomeProduto = "Monitor 27\" 144Hz",
                    Preco = 1900.00m,
                    Quantidade = 2
                },
                new()
                {
                    Id = Guid.Parse("bbbbbbbb-0002-0002-0002-000000000002"),
                    ProdutoId = ProdutoTecladoId,
                    NomeProduto = "Teclado Mecânico",
                    Preco = 350.00m,
                    Quantidade = 1
                },
            }
        };

        pedidoBruno.CalcularTotal();

        pedidoBruno.AlterarEstado(new PedidoPendenteState());

        // PEDIDO CARLA
        var pedidoCarla = new Pedido
        {
            Id = PedidoCarlaId,
            ClienteId = clienteCarla.Id,
            EnderecoEntrega = clienteCarla.Endereco,
            DataCriacao = new DateTime(2025, 4, 18),
            Itens = new List<ItemPedido>
            {
                new()
                {
                    Id = Guid.Parse("bbbbbbbb-0003-0003-0003-000000000001"),
                    ProdutoId = ProdutoWebcamId,
                    NomeProduto = "Webcam Full HD",
                    Preco = 260.00m,
                    Quantidade = 1
                },
            }
        };

        pedidoCarla.CalcularTotal();

        pedidoCarla.AlterarEstado(new PedidoPendenteState());

        // SALVANDO PEDIDOS
        db.Pedidos.AddRange(pedidoAna, pedidoBruno, pedidoCarla);

        db.SaveChanges();
    }
}
