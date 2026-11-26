using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PedidosNet.Application.Commands;
using PedidosNet.Application.Dispatchers;
using PedidosNet.Application.Handlers;
using PedidosNet.Domain;
using PedidosNet.Domain.Criacao;
using PedidosNet.Domain.Descontos;
using PedidosNet.Domain.Entities;
using PedidosNet.Domain.Validacoes;
using PedidosNet.DTOs;
using PedidosNet.Infra.Data;


namespace PedidosNet.Controllers;

[ApiController]
[Route("api/pedidos")]
public class PedidoController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPedidoFactory _pedidoFactory;
    private readonly SeledorDeDescontos _seledorDeDescontos;
    private readonly PedidoValidationChain _validationChain;
    private readonly IDispatcher _dispacher;
    

   public PedidoController(AppDbContext context, IPedidoFactory pedidoFactory, SeledorDeDescontos seledorDeDescontos
                    ,PedidoValidationChain validationChain, IDispatcher dispatcher )
    {
        _context = context;
        _pedidoFactory = pedidoFactory;
        _seledorDeDescontos = seledorDeDescontos;
        _validationChain = validationChain;
        _dispacher = dispatcher;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var pedidos = await _context.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Itens)
            .AsNoTracking()
            .ToListAsync();

        return Ok(pedidos);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Itens)
            .FirstOrDefaultAsync(p => p.Id == id);

        return pedido is null ? NotFound() : Ok(pedido);
    }

    [HttpPost]
    public async Task<IActionResult> Criar(CreatePedidoRequest request)
    {
        var cliente = await _context.Clientes.FindAsync(request.ClienteId);
        if (cliente is null)
            return BadRequest("Cliente não encontrado.");

        //Regra 1: Cliente bloqueado nem chega a criar pedido
        if(cliente.Bloqueado)
            return BadRequest("Cliente bloqueado não pode criar pedidos.");

        var itens = new List<ItemPedido>();

        foreach (var itemRequest in request.Itens)
        {
            var produto = await _context.Produtos.FindAsync(itemRequest.ProdutoId);
            if (produto is null)
                return BadRequest($"Produto {itemRequest.ProdutoId} não encontrado.");

            if (produto.Estoque < itemRequest.Quantidade)
                return BadRequest($"Estoque insuficiente para {produto.Nome}");

            itens.Add(new ItemPedido
            {
                Id = Guid.NewGuid(),
                ProdutoId = produto.Id,
                NomeProduto = produto.Nome,
                Preco = produto.Preco,
                Quantidade = itemRequest.Quantidade
            });
        }

        var totalPedido = itens.Sum(i => i.Preco * i.Quantidade);

        if (totalPedido > cliente.LimiteCredito)
        {
            return BadRequest("Limite de crédito insuficiente.");
        }

        var pedido = _pedidoFactory.Criar(cliente, itens);

        return Ok(await FinalizarPedido(pedido));
    }

    [HttpPost("kits")]
    public async Task<IActionResult> CriarKit(CreatePedidoKitRequest request)
    {
        var cliente = await _context.Clientes.FindAsync(request.ClienteId);

        if (cliente is null)
            return BadRequest("Cliente não encontrado.");

        if (cliente.Bloqueado)
            return BadRequest("Cliente bloqueado não pode criar pedidos.");

        var builder = new KitPromocionalBuilder(_pedidoFactory).ParaCliente(cliente);

        decimal totalKit = 0;

        try
        {
            foreach (var itemRequest in request.Itens)
            {
                var produto = await _context.Produtos.FindAsync(itemRequest.ProdutoId);

                if (produto is null)
                    return BadRequest($"Produto {itemRequest.ProdutoId} não encontrado.");

                if (produto.Estoque < itemRequest.Quantidade)
                    return BadRequest($"Estoque insuficiente para {produto.Nome}");

                totalKit += produto.Preco * itemRequest.Quantidade;

                builder.AddProduto(produto, itemRequest.Quantidade);
            }

            if (!string.IsNullOrWhiteSpace(request.Cupom))
                builder.AplicarCupom(request.Cupom);

            if (totalKit > cliente.LimiteCredito)
                return BadRequest("Limite de crédito insuficiente.");

            var pedido = builder.Build();

            return await FinalizarPedido(pedido);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    private async Task<IActionResult> FinalizarPedido(Pedido pedido)
    {
        var strategies = _seledorDeDescontos.Selecionar(pedido);
        pedido.AplicarDesconto(strategies);
        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();
        return Ok(pedido.Id);
    }

    [HttpPost("{id:guid}/aprovar")]
    public async Task<IActionResult> Aprovar(Guid id)
    {
        var resultado = await _dispacher.SendAsync(new AprovarPedidoCommand(id));

        if (!resultado.IsSuccess)
        {
            return Problem(
                statusCode: StatusCodes.Status422UnprocessableEntity,
                title: "Transição de estado invalida",
                detail: resultado.ErrorMensage,
                extensions: new Dictionary<string, object?>
                {
                    ["errorCode"] = resultado.ErrorCode
                });
        }
        return NoContent();
    }

    [HttpPost("{id:guid}/processar")]
    public async Task<IActionResult> Processar(Guid id)
    {
        var resultado = await _dispacher.SendAsync(new ProcessarPedidoCommand(id));

        if (!resultado.IsSuccess)
        {
            return Problem(
                statusCode: StatusCodes.Status422UnprocessableEntity,
                title: "Transição de estado invalida",
                detail: resultado.ErrorMensage,
                extensions: new Dictionary<string, object?>
                {
                    ["errorCode"] = resultado.ErrorCode
                });
        }
        return NoContent();
    }

    [HttpPost("{id:guid}/enviar")]
    public async Task<IActionResult> Enviar(Guid id)
    {
        var resultado = await _dispacher.SendAsync(new EnviarPedidoCommand(id));

        if (!resultado.IsSuccess)
        {
            return Problem(
                statusCode: StatusCodes.Status422UnprocessableEntity,
                title: "Transição de estado invalida",
                detail: resultado.ErrorMensage,
                extensions: new Dictionary<string, object?>
                {
                    ["errorCode"] = resultado.ErrorCode
                });
        }
        return NoContent();
    }

    [HttpPost("{id:guid}/entregar")]
    public async Task<IActionResult> Entregar(Guid id)
    {
        var resultado = await _dispacher.SendAsync(new EntregarPedidoCommand(id));

        if (!resultado.IsSuccess)
        {
            return Problem(
                statusCode: StatusCodes.Status422UnprocessableEntity,
                title: "Transição de estado invalida",
                detail: resultado.ErrorMensage,
                extensions: new Dictionary<string, object?>
                {
                    ["errorCode"] = resultado.ErrorCode
                });
        }
        return NoContent();
    }
}
