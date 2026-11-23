using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PedidosNet.Data;
using PedidosNet.Domain;
using PedidosNet.Domain.Criacao;
using PedidosNet.Domain.Descontos;
using PedidosNet.DTOs;
using PedidosNet.Models;

namespace PedidosNet.Controllers;

[ApiController]
[Route("api/pedidos")]
public class PedidoController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPedidoFactory _pedidoFactory;
    private readonly SeledorDeDescontos _seledorDeDescontos;

    public PedidoController(AppDbContext context, IPedidoFactory pedidoFactory, SeledorDeDescontos seledorDeDescontos)
    {
        _context = context;
        _pedidoFactory = pedidoFactory;
        _seledorDeDescontos = seledorDeDescontos;
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
        var pedido = await _context.Pedidos.FirstOrDefaultAsync(p => p.Id == id);

        if (pedido is null)
            return NotFound();

        var result = pedido.Aprovar();
        if (!result.IsSuccess)
        {
            return Problem(
                statusCode: StatusCodes.Status422UnprocessableEntity,
                title: "Transição de estado invalida",
                detail: result.ErrorMensage,
                extensions: new Dictionary<string, object?> 
                {
                    ["errorCode"] = result.ErrorCode
                });
        }
        await _context.SaveChangesAsync();
        return Ok (pedido);
    }

    [HttpPost("{id:guid}/processar")]
    public async Task<IActionResult> Processar(Guid id)
    {
        var pedido = await _context.Pedidos.FirstOrDefaultAsync(p => p.Id == id);

        if (pedido is null)
            return NotFound();

        var result = pedido.Processar();
        if (!result.IsSuccess)
        {
            return Problem(
                statusCode: StatusCodes.Status422UnprocessableEntity,
                title: "Transição de estado invalida",
                detail: result.ErrorMensage,
                extensions: new Dictionary<string, object?>
                {
                    ["errorCode"] = result.ErrorCode
                });
        }
        await _context.SaveChangesAsync();
        return Ok(pedido);
    }

    [HttpPost("{id:guid}/enviar")]
    public async Task<IActionResult> Enviar(Guid id)
    {
        var pedido = await _context.Pedidos.FirstOrDefaultAsync(p => p.Id == id);

        if (pedido is null)
            return NotFound();

        var result = pedido.Enviar();
        if (!result.IsSuccess)
        {
            return Problem(
                statusCode: StatusCodes.Status422UnprocessableEntity,
                title: "Transição de estado invalida",
                detail: result.ErrorMensage,
                extensions: new Dictionary<string, object?>
                {
                    ["errorCode"] = result.ErrorCode
                });
        }
        await _context.SaveChangesAsync();
        return Ok(pedido);
    }

    [HttpPost("{id:guid}/entregar")]
    public async Task<IActionResult> Entregar(Guid id)
    {
        var pedido = await _context.Pedidos.FirstOrDefaultAsync(p => p.Id == id);

        if (pedido is null)
            return NotFound();

        var result = pedido.Entregar();
        if (!result.IsSuccess)
        {
            return Problem(
                statusCode: StatusCodes.Status422UnprocessableEntity,
                title: "Transição de estado invalida",
                detail: result.ErrorMensage,
                extensions: new Dictionary<string, object?>
                {
                    ["errorCode"] = result.ErrorCode
                });
        }
        await _context.SaveChangesAsync();
        return Ok(pedido);
    }
}
