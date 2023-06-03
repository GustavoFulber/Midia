using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Admin.Controllers
{

    [ApiController]
    public class MidiaController : ControllerBase
    {
        private readonly IWebHostEnvironment _ambiente;

        private readonly long _limiteTamanhoArquivo;

        private readonly DataContext _contexto;

        public MidiaController(IWebHostEnvironment ambiente, IConfiguration configuracao, DataContext contexto)
        {
            _ambiente = ambiente;
            _limiteTamanhoArquivo = configuracao.GetValue<long>("FileUploadLimit");
            _contexto = contexto;
        }

        [Route("buscaArquivosUsuario/{codigo}")]
        [HttpGet]
        public async Task<IActionResult> buscaArquivosUsuario(string codigo)
        {
            var usuarios = await _contexto.Usuario
                                 .Where(u => u.Codigo == codigo)
                                 .Include(u => u.Arquivos)
                                 .ToListAsync();

            if (!usuarios.Any())
            {
                return NotFound("Usuário não encontrado");
            }

            var arquivosUsuarios = new List<object>();

            foreach (var usuario in usuarios)
            {
                var arquivosUsuario = usuario.Arquivos.Select(a => new
                {
                    a.ArquivoId,
                    a.Nome,
                    a.Tipo,
                    a.Url,
                    a.DataCriacao,
                    a.DataAtualizacao,
                    a.Deletado
                });

                arquivosUsuarios.AddRange(arquivosUsuario);
            }

            return Ok(arquivosUsuarios);
        }

        [Route("salvarArquivo/{codigo}")]
        [HttpPost]
        public async Task<IActionResult> salvarArquivo(string codigo, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Nenhum Arquivo Enviado");
            }

            if (file.Length > _limiteTamanhoArquivo)
            {
                return BadRequest("Tamanho Maximo de 100MB");
            }

            if (string.IsNullOrEmpty(codigo)) { return BadRequest("Código não inserido!"); }

            var usuario = await _contexto.Usuario.FirstOrDefaultAsync(u => u.Codigo == codigo);

            if (usuario == null)
            {
                return NotFound("Usuário não encontrado");
            }

            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            var fileExtension = Path.GetExtension(file.FileName);
            var fileContentType = file.ContentType;

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var fileBytes = memoryStream.ToArray();

            string uniqueUrl;
            do
            {
                var randomString = Utils.StringAleatoria.GeraString(12);
                uniqueUrl = $"{randomString}{fileName}{fileExtension}";
            } while (await _contexto.Arquivo.AnyAsync(x => x.Url == uniqueUrl));

            var fileRecord = new Arquivo(
                    fileName,
                    fileBytes,
                    fileContentType,
                    uniqueUrl,
                    DateTime.UtcNow,
                    DateTime.UtcNow,
                    false
                );

            fileRecord.Usuario = new List<global::Usuario> { usuario };

            _contexto.Arquivo.Add(fileRecord);
            await _contexto.SaveChangesAsync();

            return Ok(new SalvaArquivo(uniqueUrl));
        }

        [Route("salvarArquivoNome/{nomeArquivo}/{codigo}")]
        [HttpPost]
        public async Task<IActionResult> salvarArquivoNome(string codigo, IFormFile file, string nomeArquivo)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Nenhum Arquivo Enviado");
            }

            if (file.Length > _limiteTamanhoArquivo)
            {
                return BadRequest("Tamanho Maximo de 100MB");
            }

            if (string.IsNullOrEmpty(codigo)) { return BadRequest("Código não inserido!"); }

            var usuario = await _contexto.Usuario.FirstOrDefaultAsync(u => u.Codigo == codigo);

            if (usuario == null)
            {
                return NotFound("Usuárop não encontrado");
            }

            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            var fileExtension = Path.GetExtension(file.FileName);
            var fileContentType = file.ContentType;

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var fileBytes = memoryStream.ToArray();

            string uniqueUrl;
            do
            {
                var randomString = Utils.StringAleatoria.GeraString(12);
                uniqueUrl = $"{randomString}{nomeArquivo}{fileExtension}";
            } while (await _contexto.Arquivo.AnyAsync(x => x.Url == uniqueUrl));

            var fileRecord = new Arquivo(
                fileName,
                fileBytes,
                fileContentType,
                uniqueUrl,
                DateTime.UtcNow,
                DateTime.UtcNow,
                false
            );

            fileRecord.Usuario = new List<global::Usuario> { usuario };

            _contexto.Arquivo.Add(fileRecord);
            await _contexto.SaveChangesAsync();

            return Ok(new SalvaArquivo(uniqueUrl));
        }

        [Route("alterarNomeArquivo/{url}/{codigo}")]
        [HttpPatch]
        public async Task<IActionResult> AlterarNomeArquivoUrl(string codigo, string url, [FromBody] string novoNome)
        {

            if (string.IsNullOrEmpty(codigo)) { return BadRequest("Código não inserido!"); }

            var registroArquivo = await _contexto.Arquivo.FirstOrDefaultAsync(x => x.Url == url);

            if (registroArquivo == null)
            {
                return NotFound();
            }

            if (!registroArquivo.Usuario.Any(u => u.Codigo == codigo))
            {
                return BadRequest("O arquivo não está associado ao Usuário com o Código especificado");
            }

            registroArquivo.Nome = novoNome;
            registroArquivo.DataAtualizacao = DateTime.UtcNow;

            _contexto.Arquivo.Update(registroArquivo);
            await _contexto.SaveChangesAsync();

            return Ok();
        }

        [Route("editarArquivo/{url}/{codigo}")]
        [HttpPut]
        public async Task<IActionResult> EditarArquivoUrl(string codigo, string url, IFormFile arquivo)
        {
            if (string.IsNullOrEmpty(codigo)) { return BadRequest("Código não inserido!"); }

            var registroArquivo = await _contexto.Arquivo.FirstOrDefaultAsync(x => x.Url == url);

            if (registroArquivo == null)
            {
                return NotFound();
            }

            if (!registroArquivo.Usuario.Any(u => u.Codigo == codigo))
            {
                return BadRequest("O arquivo não está associado ao Usuário com o Código especificado");
            }

            if (arquivo != null && arquivo.Length > 0 && arquivo.Length <= _limiteTamanhoArquivo)
            {
                using var memoryStream = new MemoryStream();
                await arquivo.CopyToAsync(memoryStream);

                registroArquivo.Dado = memoryStream.ToArray();
                registroArquivo.Tipo = arquivo.ContentType;
                registroArquivo.DataAtualizacao = DateTime.UtcNow;
            }

            _contexto.Arquivo.Update(registroArquivo);
            await _contexto.SaveChangesAsync();

            return Ok();
        }

        [Route("visualizarInformacoesArquivo/{url}/{codigo}")]
        [HttpGet]
        public async Task<IActionResult> VisualizarInformacoesArquivoUrl(string codigo, string url)
        {
            if (string.IsNullOrEmpty(codigo)) { return BadRequest("Código não inserido!"); }

            var arquivo = await _contexto.Arquivo.FirstOrDefaultAsync(x => x.Url == url);

            if (arquivo == null)
            {
                return NotFound();
            }

            if (!arquivo.Usuario.Any(u => u.Codigo == codigo))
            {
                return BadRequest("O arquivo não está associado ao Usuário com o Código especificado");
            }

            return Ok(new
            {
                arquivo.ArquivoId,
                arquivo.Nome,
                arquivo.Tipo,
                arquivo.Url,
                arquivo.DataCriacao,
                arquivo.DataAtualizacao,
                arquivo.Deletado
            });
        }

        [Route("baixarArquivo/{url}/{codigo}")]
        [HttpGet]
        public async Task<IActionResult> DownloadFile(string codigo, string url)
        {
            if (string.IsNullOrEmpty(codigo)) { return BadRequest("Código não inserido!"); }

            var arquivo = await _contexto.Arquivo.FirstOrDefaultAsync(x => x.Url == url);

            if (arquivo == null)
            {
                return NotFound();
            }

            if (!arquivo.Usuario.Any(u => u.Codigo == codigo))
            {
                return BadRequest("O arquivo não está associado ao Usuário com o Código especificado");
            }

            var fileStream = new MemoryStream(arquivo.Dado);

            return new FileStreamResult(fileStream, arquivo.Tipo)
            {
                FileDownloadName = arquivo.Nome
            };
        }

        [Route("verArquivo/{url}/{codigo}")]
        [HttpGet]
        public async Task<IActionResult> ViewFile(string codigo, string url)
        {
            if (string.IsNullOrEmpty(codigo)) { return BadRequest("Código não inserido!"); }

            var arquivo = await _contexto.Arquivo.FirstOrDefaultAsync(x => x.Url == url);

            if (arquivo == null)
            {
                return NotFound();
            }

            if (!arquivo.Usuario.Any(u => u.Codigo == codigo))
            {
                return BadRequest("O arquivo não está associado ao Usuário com o Código especificado");
            }

            var fileStream = new MemoryStream(arquivo.Dado);

            var result = new FileStreamResult(fileStream, arquivo.Tipo)
            {
                FileDownloadName = arquivo.Nome
            };

            Response.Headers.Add("Content-Disposition", $"inline; filename={arquivo.Nome}");

            return result;
        }

        [Route("deletarArquivo/{url}/{codigo}")]
        [HttpDelete]
        public async Task<IActionResult> DeletarArquivoPorUrl(string codigo, string url)
        {
            if (string.IsNullOrEmpty(codigo)) { return BadRequest("Código não inserido!"); }

            var arquivo = await _contexto.Arquivo.FirstOrDefaultAsync(a => a.Url == url);

            if (arquivo == null)
            {
                return NotFound("Arquivo não encontrado");
            }

            if (!arquivo.Usuario.Any(u => u.Codigo == codigo))
            {
                return BadRequest("O arquivo não está associado ao Usuário com o Código especificado");
            }

            arquivo.Deletado = true;
            arquivo.DataAtualizacao = DateTime.UtcNow;

            await _contexto.SaveChangesAsync();

            return Ok();
        }

        [Route("deletarFisicamenteArquivo/{url}/{codigo}")]
        [HttpDelete]
        public async Task<IActionResult> DeletarFisicamenteArquivoPorUrl(string codigo, string url)
        {
            if (string.IsNullOrEmpty(codigo)) { return BadRequest("Código não inserido!"); }

            var arquivo = await _contexto.Arquivo.Include(a => a.Usuario).FirstOrDefaultAsync(a => a.Url == url);

            if (arquivo == null)
            {
                return NotFound("Arquivo não encontrado");
            }

            if (!arquivo.Usuario.Any(u => u.Codigo == codigo))
            {
                return BadRequest("O arquivo não está associado ao Usuário com o Código especificado");
            }

            _contexto.Arquivo.Remove(arquivo);

            await _contexto.SaveChangesAsync();

            return Ok();
        }
    }
}