
using System.Text.RegularExpressions;

namespace PedidosNet.Models
{
    public sealed record Endereco
    {
        public string Cep { get; private set; }
        public string Logradouro { get; private set; }
        public string Numero { get; private set; }
        public string Complemento { get; private set; }
        public string Bairro { get; private set; }
        public string Cidade { get; private set; }
        public string Estado { get; private set; }

        public Endereco(
            string cep,
            string logradouro,
            string numero,
            string bairro,
            string cidade,
            string estado,
            string complemento = "")
        {
            ValidarCep(cep);

            ValidarCampo(logradouro, nameof(logradouro));
            ValidarCampo(numero, nameof(numero));
            ValidarCampo(bairro, nameof(bairro));
            ValidarCampo(cidade, nameof(cidade));
            ValidarCampo(estado, nameof(estado));

            Cep = cep;
            Logradouro = logradouro;
            Numero = numero;
            Complemento = complemento;
            Bairro = bairro;
            Cidade = cidade;
            Estado = estado;
        }

        private static void ValidarCampo(string valor, string nome)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException($"{nome} é obrigatório.", nome);
        }

        private static void ValidarCep(string cep)
        {
            if (string.IsNullOrWhiteSpace(cep) ||
                !Regex.IsMatch(cep, @"^\d{5}-\d{3}$"))
            {
                throw new ArgumentException(
                    "CEP inválido. Formato esperado: 00000-000.",
                    nameof(cep));
            }
        }
    }
}
