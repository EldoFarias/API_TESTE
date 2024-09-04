using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WFConFin.Dados;
using WFConFin.Models;
using WFConFin.Servicos;

namespace WFConFin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize] // Com essa anotação eu digo que para qualquer metodo precisa de autenticação.
    public class UsuarioController : Controller
    {

        private readonly WFConfinDBContext _context;
        private readonly TokenService _service;

        public UsuarioController(TokenService service, WFConfinDBContext dbContext)
        {
            _service = service;
            _context = dbContext;
        }

        //Metodo para logar o usuario e gerar o token
        [HttpPost]
        [Route("Login")]
        [AllowAnonymous] // Mesmo sem autenticação eu posso acessar aqui
        public async Task<IActionResult> LoginUsuario([FromBody] UsuarioLogin usuarioLogin)
        {

            string senhaHash = MD5Hash.CalcularHash(usuarioLogin.Password);
            
            var usuario =  _context.Usuario.Where(x => x.Login == usuarioLogin.Login).FirstOrDefault();
            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }
            else if (usuario.Password != senhaHash)
            {
                return BadRequest("A senha digitada está incorreta.");
            }
            else
            {

                var token = _service.GerarToken(usuario);

                if (token != null)
                {
                    usuario.Password = "";

                    var resultado = new UsuarioResposta()
                    {
                        Usuario = usuario,
                        Token = token
                    };

                    return Ok(resultado);

                }
                else
                {
                    return BadRequest("Não foi possível obter o token.");
                }

            }

        }

        //Abaixo estão os metodos de CRUD basicos

        [HttpGet]
        [Authorize(Roles = "Gerente, Empregado")]
        public async Task<IActionResult> GetListarUsuario()
        {
            try
            {
                var result = await _context.Usuario.ToListAsync();
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na listagem de usuarios.{ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> PostSalvarUsuario([FromBody] Usuario usuario)
        {

            try
            {

                var listaUsuario = _context.Usuario.Where(x => x.Login == usuario.Login).ToList();

                if (listaUsuario.Count > 0)
                {
                    return BadRequest("Já existe um usuário com este nome de login.");
                }

                string senhaHash = MD5Hash.CalcularHash(usuario.Password);
                usuario.Password = senhaHash;


                var resulta = await _context.Usuario.AddAsync(usuario);
                var valor = await _context.SaveChangesAsync();

                if (valor == 1)
                {
                    return Ok("Sucesso ao salvar a usuario.");
                }
                else
                {
                    return BadRequest($"Erro ao gravar usuario.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao salvar usuario: {ex} ");
            }

        }

        [HttpPut]
        [Authorize(Roles = "Gerente, Empregado")]
        public async Task<IActionResult> PutAtualizarUsuario([FromBody] Usuario usuario)
        {

            try
            {
                var resulta = _context.Usuario.Update(usuario);
                var valor = await _context.SaveChangesAsync();

                if (valor == 1)
                {
                    return Ok("Sucesso ao atualizar usuario.");
                }
                else
                {
                    return BadRequest($"Erro ao atualizar usuario.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar usuario: {ex} ");
            }

        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> DeleteUsuario([FromRoute] Guid id)
        {
            try
            {
                // Verifica se o ID é o valor padrão (00000000-0000-0000-0000-000000000000)
                if (id == Guid.Empty)
                {
                    return BadRequest("ID inválido.");
                }

                // Busca a pessoa pelo ID
                var usuario = await _context.Usuario.FindAsync(id);

                if (usuario == null)
                {
                    return NotFound("Usuario não encontrada."); // Use NotFound para indicar que o recurso não foi encontrado
                }

                // Remove a pessoa
                _context.Usuario.Remove(usuario);
                var valor = await _context.SaveChangesAsync();

                if (valor > 0) // Salva as mudanças no banco de dados
                {
                    return Ok("Usuário deletada com sucesso.");
                }
                else
                {
                    return BadRequest("Erro ao remover Usuário.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao deletar Usuário: {ex.Message}"); // Retorna código de erro 500 para erros internos
            }
        }
    }
}
