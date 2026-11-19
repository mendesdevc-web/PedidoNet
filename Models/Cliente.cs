namespace PedidosNet.Models;

public class Cliente
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Endereco Endereco { get; set; } = null!;
    public SegmentoCliente Segmento { get; private set; }
    public bool Bloqueado { get; set; }
    public DateTime DataCadastro { get; set; }


    public Cliente()
    {
    }

    public Cliente(Guid id, string nome ,string email ,Endereco endereco, 
        SegmentoCliente segmento, DateTime dataCadastro, bool bloqueado = false)
    {
        Id = id;
        Nome = nome;
        Email = email;
        Endereco = endereco;
        Segmento = segmento;
        DataCadastro = dataCadastro;
        Bloqueado = bloqueado;
    }
}
