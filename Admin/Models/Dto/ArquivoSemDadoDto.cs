public class ArquivoSemDadoDto
{
    public int ArquivoId { get; set; }

    public string Nome { get; set; }

    public string Tipo { get; set; }

    public string Url { get; set; }

    public DateTime DataCriacao { get; set; }

    public DateTime DataAtualizacao { get; set; }

    public bool Deletado { get; set; }

    public ArquivoSemDadoDto(int arquivoId, string nome, byte[] dado, string tipo, string url, DateTime dataCriacao, DateTime dataAtualizacao, bool deletado)
    {
        ArquivoId = arquivoId;
        Nome = nome;
        Tipo = tipo;
        Url = url;
        DataCriacao = dataCriacao;
        DataAtualizacao = dataAtualizacao;
        Deletado = deletado;
    }
}