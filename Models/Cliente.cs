namespace PedidosNet.Models;

public class Cliente
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public bool Vip { get; set; }
    public bool Corporativo { get; set; }
    public bool Bloqueado { get; set; }
    public DateTime DataCadastro { get; set; }
}
