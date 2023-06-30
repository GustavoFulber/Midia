using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Admin.Controllers
{
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly DataContext _contexto;

        public UsuarioController(DataContext contexto)
        {
            _contexto = contexto;
        }

        [Route("salvarUsuario/{codigo}")]
        [HttpPost]
        public async Task<IActionResult> SalvarUsuario(string codigo)
        {
            var usuarioExistente = await _contexto.Usuario.AnyAsync(u => u.Codigo == codigo);

            if (usuarioExistente)
            {
                return BadRequest("Um usuário com este código já existe");
            }

            var usuario = new Usuario
            {
                Codigo = codigo,
                Deletado = false
            };

            _contexto.Usuario.Add(usuario);
            await _contexto.SaveChangesAsync();

            return Ok();
        }

        [Route("validarUsuario/{codigo}")]
        [HttpGet]
        public async Task<IActionResult> ValidarUsuario(string codigo)
        {
            var usuarioExistente = await _contexto.Usuario.AnyAsync(u => u.Codigo == codigo);

            if (!usuarioExistente)
            {
                return Ok(false);
            }

            return Ok(true);
        }
    }
}
