using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WFConFin.Dados;
using WFConFin.Models;

namespace WFConFin.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Com essa anotação eu digo que para qualquer metodo precisa de autenticação.
    public class ContaController : Controller
    {

        private readonly WFConfinDBContext _context;

        public ContaController(WFConfinDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Gerente, Empregado")]
        public async Task<IActionResult> GetBuscarTodasContas()
        {
            try
            {
                var resultado = await _context.Conta
                                              .Include(c => c.Pessoa)  // Inclui a entidade Pessoa
                                              .ToListAsync();
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao listar contas: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Gerente, Empregado")]
        public async Task<IActionResult> PostSalvarConta([FromBody] Conta conta)
        {

            try
            {
                var resulta = await _context.Conta.AddAsync(conta);
                var valor = await _context.SaveChangesAsync();

                if (valor == 1)
                {
                    return Ok("Sucesso ao salvar a conta.");
                }
                else
                {
                    return BadRequest($"Erro ao gravar conta.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao salvar conta: {ex} ");
            }

        }

        [HttpPut]
        [Authorize(Roles = "Gerente, Empregado")]
        public async Task<IActionResult> PutAtualizarConta([FromBody] Conta conta)
        {

            try
            {
                var resulta = _context.Conta.Update(conta);
                var valor = await _context.SaveChangesAsync();

                if (valor == 1)
                {
                    return Ok("Sucesso ao atualizar conta.");
                }
                else
                {
                    return BadRequest($"Erro ao atualizar conta.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar conta: {ex} ");
            }

        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Gerente")]
        public async Task<IActionResult> DeleteConta([FromRoute] Guid id)
        {
            try
            {
                // Verifica se o ID é o valor padrão (00000000-0000-0000-0000-000000000000)
                if (id == Guid.Empty)
                {
                    return BadRequest("ID inválido.");
                }

                // Busca a pessoa pelo ID
                var conta = await _context.Conta.FindAsync(id);

                if (conta == null)
                {
                    return NotFound("Conta não encontrada."); // Use NotFound para indicar que o recurso não foi encontrado
                }

                // Remove a pessoa
                _context.Conta.Remove(conta);
                var valor = await _context.SaveChangesAsync();

                if (valor > 0) // Salva as mudanças no banco de dados
                {
                    return Ok("Conta deletada com sucesso.");
                }
                else
                {
                    return BadRequest("Erro ao remover conta.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao deletar conta: {ex.Message}"); // Retorna código de erro 500 para erros internos
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Gerente, Empregado")]
        public async Task<IActionResult> GetBuscarContaUnica([FromRoute] Guid id)
        {
            try
            {
                // Verifica se o ID é o valor padrão (00000000-0000-0000-0000-000000000000)
                if (id == Guid.Empty)
                {
                    return BadRequest("ID inválido.");
                }

                // Busca a conta pelo ID e inclui a entidade Pessoa relacionada
                var conta = await _context.Conta
                                          .Include(c => c.Pessoa) // Inclui a entidade Pessoa
                                          .FirstOrDefaultAsync(c => c.Id == id);

                if (conta == null)
                {
                    return NotFound("Conta não encontrada."); // Retorna 404 se a conta não for encontrada
                }

                return Ok(conta); // Retorna a conta encontrada com status 200 (OK)
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar conta: {ex.Message}"); // Retorna 500 em caso de erro interno
            }
        }

        [HttpGet("Pesquisa")]
        [Authorize(Roles = "Gerente, Empregado")]
        public async Task<IActionResult> GetContaPesquisa([FromQuery] string valor)
        {
            try
            {
                if (valor != null)
                {

                    var lista = from o in _context.Conta.Include(o => o.Pessoa).ToList()
                                where o.Descricao.ToUpper().Contains(valor.ToUpper()) ||
                                o.Pessoa.Nome.ToUpper().Contains(valor.ToUpper())
                                select o;

                    return Ok(lista);

                }
                else
                {
                    return BadRequest("Informe algum valor na pesquisa.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao consultar conta: {ex.Message}");
            }
        }

        [HttpGet("Paginacao")]
        [Authorize(Roles = "Gerente, Empregado")]
        public async Task<IActionResult> GetContaPaginacao([FromQuery] string valor, int skip, int take, bool ordemDesc)
        {
            try
            {
                // Valida os parâmetros
                var resultadoValidacao = ValidarParametrosPaginacao(valor, skip, take);
                if (!string.IsNullOrEmpty(resultadoValidacao))
                {
                    return BadRequest(resultadoValidacao);
                }

                // Usando o Expression
                Expression<Func<Conta, bool>> expressao = o =>
                    o.Descricao.ToUpper().Contains(valor.ToUpper());

                // Aplica o filtro
                var query = _context.Conta.Where(expressao);

                // Aplica a ordenação
                query = ordemDesc
                    ? query.OrderByDescending(o => o.Descricao)
                    : query.OrderBy(o => o.Descricao);

                // Conta o total de registros antes da paginação
                var qtde = await query.CountAsync();

                // Aplica a paginação
                var lista = await query
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync();

                // Prepara a resposta de paginação
                var paginacaoResponse = new PaginacaoResponse<Conta>(lista, qtde, skip, take);

                return Ok(paginacaoResponse);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao pesquisar contas. {ex.Message}");
            }
        }

        private string ValidarParametrosPaginacao(string valor, int skip, int take)
        {
            if (string.IsNullOrEmpty(valor))
            {
                return "O parâmetro 'valor' é obrigatório e não pode estar vazio.";
            }

            if (skip < 0)
            {
                return "O parâmetro 'skip' não pode ser menor que zero.";
            }

            if (take <= 0)
            {
                return "O parâmetro 'take' deve ser maior que zero.";
            }

            return string.Empty; // Parâmetros válidos
        }

    }
}
