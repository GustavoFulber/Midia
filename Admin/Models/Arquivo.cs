using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Arquivo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ArquivoId { get; set; }

    [MaxLength(255, ErrorMessage = "{0} deve ter no maximo {1} caractares.")]
    public string Nome { get; set; }

    public byte[] Dado { get; set; }

    [MaxLength(255, ErrorMessage = "{0} deve ter no maximo {1} caractares.")]
    public string Tipo { get; set; }

    [MaxLength(255, ErrorMessage = "{0} deve ter no maximo {1} caractares.")]
    public string Url { get; set; }

    public DateTime DataCriacao { get; set; }

    public DateTime DataAtualizacao { get; set; }

    public bool Deletado { get; set; }

    public Arquivo(int arquivoId, string nome, byte[] dado, string tipo, string url, DateTime dataCriacao, DateTime dataAtualizacao, bool deletado)
    {
        ArquivoId = arquivoId;
        Nome = nome;
        Dado = dado;
        Tipo = tipo;
        Url = url;
        DataCriacao = dataCriacao;
        DataAtualizacao = dataAtualizacao;
        Deletado = deletado;
    }

    public Arquivo(string nome, byte[] dado, string tipo, string url, DateTime dataCriacao, DateTime dataAtualizacao, bool deletado)
    {
        ArquivoId = 0;
        Nome = nome;
        Dado = dado;
        Tipo = tipo;
        Url = url;
        DataCriacao = dataCriacao;
        DataAtualizacao = dataAtualizacao;
        Deletado = deletado;
    }
}