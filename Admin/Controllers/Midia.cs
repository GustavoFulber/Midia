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

        [Route("salvarArquivo")]
        [HttpPost]
        public async Task<IActionResult> salvarArquivo(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Nenhum Arquivo Enviado");
            }

            if (file.Length > _limiteTamanhoArquivo)
            {
                return BadRequest("Tamanho Maximo de 100MB");
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


            _contexto.Arquivo.Add(fileRecord);
            await _contexto.SaveChangesAsync();

            return Ok(new SalvaArquivo(uniqueUrl));
        }

        [Route("salvarArquivoNome/{nomeArquivo}")]
        [HttpPost]
        public async Task<IActionResult> salvarArquivoNome(IFormFile file, string nomeArquivo)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Nenhum Arquivo Enviado");
            }

            if (file.Length > _limiteTamanhoArquivo)
            {
                return BadRequest("Tamanho Maximo de 100MB");
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

            _contexto.Arquivo.Add(fileRecord);
            await _contexto.SaveChangesAsync();

            return Ok(new SalvaArquivo(uniqueUrl));
        }

        [Route("alterarNomeArquivo/{url}")]
        [HttpPatch]
        public async Task<IActionResult> AlterarNomeArquivoUrl(string url, [FromBody] string novoNome)
        {
            var registroArquivo = await _contexto.Arquivo.Where(x => x.Url == url).FirstOrDefaultAsync();

            if (registroArquivo == null)
            {
                return NotFound();
            }

            registroArquivo.Nome = novoNome;
            registroArquivo.DataAtualizacao = DateTime.UtcNow;

            _contexto.Arquivo.Update(registroArquivo);
            await _contexto.SaveChangesAsync();

            return Ok();
        }

        [Route("editarArquivo/{url}")]
        [HttpPut]
        public async Task<IActionResult> EditarArquivoUrl(string url, IFormFile arquivo)
        {
            var registroArquivo = await _contexto.Arquivo.Where(x => x.Url == url).FirstOrDefaultAsync(); ;

            if (registroArquivo == null)
            {
                return NotFound();
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

        [Route("visualizarInformacoesArquivo/{url}")]
        [HttpGet]
        public async Task<IActionResult> VisualizarInformacoesArquivoUrl(string url)
        {
            var registroArquivo = await _contexto.Arquivo.Where(x => x.Url == url).FirstOrDefaultAsync();

            if (registroArquivo == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                registroArquivo.ArquivoId,
                registroArquivo.Nome,
                registroArquivo.Tipo,
                registroArquivo.Url,
                registroArquivo.DataCriacao,
                registroArquivo.DataAtualizacao,
                registroArquivo.Deletado
            });
        }

        [Route("baixarArquivo/{url}")]
        [HttpGet]
        public async Task<IActionResult> DownloadFile(string url)
        {
            var fileRecord = await _contexto.Arquivo.Where(x => x.Url == url).FirstOrDefaultAsync();

            if (fileRecord == null)
            {
                return NotFound();
            }

            var fileStream = new MemoryStream(fileRecord.Dado);

            return new FileStreamResult(fileStream, fileRecord.Tipo)
            {
                FileDownloadName = fileRecord.Nome
            };
        }

        [Route("verArquivo/{url}")]
        [HttpGet]
        public async Task<IActionResult> ViewFile(string url)
        {
            var fileRecord = await _contexto.Arquivo.Where(x => x.Url == url).FirstOrDefaultAsync();

            if (fileRecord == null)
            {
                return NotFound();
            }

            var fileStream = new MemoryStream(fileRecord.Dado);

            var result = new FileStreamResult(fileStream, fileRecord.Tipo)
            {
                FileDownloadName = fileRecord.Nome
            };

            Response.Headers.Add("Content-Disposition", $"inline; filename={fileRecord.Nome}");

            return result;
        }

        [Route("deletarArquivo/{url}")]
        [HttpDelete]
        public async Task<IActionResult> DeletarArquivoPorUrl(string url)
        {
            var arquivo = await _contexto.Arquivo.FirstOrDefaultAsync(a => a.Url == url);
            if (arquivo == null)
            {
                return NotFound();
            }

            arquivo.Deletado = true;
            arquivo.DataAtualizacao = DateTime.UtcNow;

            await _contexto.SaveChangesAsync();

            return Ok();
        }

        [Route("deletarFisicamenteArquivo/{url}")]
        [HttpDelete]
        public async Task<IActionResult> DeletarFisicamenteArquivoPorUrl(string url)
        {
            var arquivo = await _contexto.Arquivo.FirstOrDefaultAsync(a => a.Url == url);
            if (arquivo == null)
            {
                return NotFound();
            }

            _contexto.Arquivo.Remove(arquivo);

            await _contexto.SaveChangesAsync();

            return Ok();
        }
    }
}