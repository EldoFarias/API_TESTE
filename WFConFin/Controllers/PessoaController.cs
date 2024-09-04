using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WFConFin.Dados;
using WFConFin.Models;

namespace WFConFin.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class PessoaController : Controller
    {


        private readonly WFConfinDBContext _context;

        public PessoaController(WFConfinDBContext context)
        {
            _context = context;
        }



        [HttpGet]
        public async Task<IActionResult> GetAllPessoas()
        {
            try
            {
                var resultado = await _context.Pessoa.ToListAsync();
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao listar pessoas: {ex.Message}");
            }
        }


        [HttpPost]
        public async Task<IActionResult> PostPessoa([FromBody] Pessoa pessoa)
        {

            try
            {
                var resulta = await _context.Pessoa.AddAsync(pessoa);
                var valor = await _context.SaveChangesAsync();

                if (valor == 1)
                {
                    return Ok("Sucesso ao salvar a pessoa.");
                }
                else
                {
                    return BadRequest($"Erro ao gravar pessoa.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao salvar pessoa: {ex} ");
            }

        }


        [HttpPut]
        public async Task<IActionResult> PutPessoa([FromBody] Pessoa pessoa)
        {

            try
            {
                var resulta = _context.Pessoa.Update(pessoa);
                var valor = await _context.SaveChangesAsync();

                if (valor == 1)
                {
                    return Ok("Sucesso ao atualizar pessoa.");
                }
                else
                {
                    return BadRequest($"Erro ao atualizar pessoa.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar pessoa: {ex} ");
            }

        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePessoa([FromRoute] Guid id)
        {
            try
            {
                // Verifica se o ID é o valor padrão (00000000-0000-0000-0000-000000000000)
                if (id == Guid.Empty)
                {
                    return BadRequest("ID inválido.");
                }

                // Busca a pessoa pelo ID
                var pessoa = await _context.Pessoa.FindAsync(id);

                if (pessoa == null)
                {
                    return NotFound("Pessoa não encontrada."); // Use NotFound para indicar que o recurso não foi encontrado
                }

                // Remove a pessoa
                _context.Pessoa.Remove(pessoa);
                var valor = await _context.SaveChangesAsync();

                if (valor > 0) // Salva as mudanças no banco de dados
                {
                    return Ok("Pessoa deletada com sucesso.");
                }
                else
                {
                    return BadRequest("Erro ao remover pessoa.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao deletar pessoa: {ex.Message}"); // Retorna código de erro 500 para erros internos
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetPessoa([FromRoute] Guid id)
        {
            try
            {
                // Verifica se o ID é o valor padrão (00000000-0000-0000-0000-000000000000)
                if (id == Guid.Empty)
                {
                    return BadRequest("ID inválido.");
                }

                // Busca a pessoa pelo ID
                var pessoa = await _context.Pessoa.FindAsync(id);

                if (pessoa == null)
                {
                    return NotFound("Pessoa não encontrada."); // Retorna 404 se a pessoa não for encontrada
                }

                return Ok(pessoa); // Retorna a pessoa encontrada com status 200 (OK)
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar pessoa: {ex.Message}"); // Retorna 500 em caso de erro interno
            }
        }

        [HttpGet("Pesquisa")]
        public async Task<IActionResult> GetPessoaPesquisa([FromQuery] string valor)
        {
            try
            {

                if (valor != null)
                {

                    //Usando o Expression
                    Expression<Func<Pessoa, bool>> expressao = o => true;
                    expressao = o => o.Nome.ToUpper().Contains(valor.ToUpper())

                            || o.Genero.ToUpper().Contains(valor.ToUpper())
                            || o.Telefone.ToUpper().Contains(valor.ToUpper())
                            || o.Email.ToUpper().Contains(valor.ToUpper());

                    var lista = await _context.Pessoa.Where(expressao).ToListAsync();
                    return Ok(lista);

                }
                else
                {
                    return BadRequest("Informe algum valor na pesquisa.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao consultar pessoa.{ex.Message}");
            }
        }




        [HttpGet("Paginacao")]
        public async Task<IActionResult> GetPessoaPaginacao([FromQuery] string valor, int skip, int take, bool ordemDesc)
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
                Expression<Func<Pessoa, bool>> expressao = o =>
                    o.Nome.ToUpper().Contains(valor.ToUpper())
                    || o.Genero.ToUpper().Contains(valor.ToUpper())
                    || o.Telefone.ToUpper().Contains(valor.ToUpper())
                    || o.Email.ToUpper().Contains(valor.ToUpper());

                // Aplica o filtro
                var query = _context.Pessoa.Where(expressao);

                // Aplica a ordenação
                query = ordemDesc
                    ? query.OrderByDescending(o => o.Nome)
                    : query.OrderBy(o => o.Nome);

                // Conta o total de registros antes da paginação
                var qtde = await query.CountAsync();

                // Aplica a paginação
                var lista = await query
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync();

                // Prepara a resposta de paginação
                var paginacaoResponse = new PaginacaoResponse<Pessoa>(lista, qtde, skip, take);

                return Ok(paginacaoResponse);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao pesquisar pessoas. {ex.Message}");
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
