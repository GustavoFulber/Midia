using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Usuario
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [MaxLength(255, ErrorMessage = "{0} deve ter no maximo {1} caractares.")]
    public string? Codigo { get; set; }

    public ICollection<Arquivo>? Arquivos { get; set; }

    public bool Deletado { get; set; }

}
