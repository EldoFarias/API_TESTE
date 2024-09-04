using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using WFConFin.Dados;
using WFConFin.Models;

namespace WFConFin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CidadeController : Controller
    {

        private readonly WFConfinDBContext _context;
        public CidadeController(WFConfinDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCidade()
        {
            try
            {
                var result = await _context.Cidade.Include(x => x.Estado).ToListAsync(); //Include é o~pcional
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na listagem das cidades.{ex.Message}");
            }
        }


        [HttpPost]
        public async Task<IActionResult> PostCidade([FromBody] Cidade cidade)
        {
            try
            {
                await _context.Cidade.AddAsync(cidade);
                var valor = _context.SaveChanges();

                if (valor == 1)
                {
                    return Ok("Sucesso ao salvar a cidade.");
                }
                else
                {
                    return BadRequest($"Erro ao gravar cidades.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao gravar cidades.{ex.Message}");
            }
        }



        [HttpPut]
        public async Task<IActionResult> PutCidade([FromBody] Cidade cidade)
        {
            try
            {
                _context.Cidade.Update(cidade);
                var valor = await _context.SaveChangesAsync();

                if (valor == 1)
                {
                    return Ok("Sucesso ao alterar a cidade.");
                }
                else
                {
                    return BadRequest($"Erro ao alterar cidades.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao alterar cidades.{ex.Message}");
            }
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCidade([FromRoute] Guid id)
        {
            try
            {

                Cidade cidade = await _context.Cidade.FindAsync(id);
                if (cidade != null)
                {
                    _context.Cidade.Remove(cidade);
                    var valor = await _context.SaveChangesAsync();
                    if (valor == 1)
                    {
                        return Ok("Sucesso ao remover a cidade.");
                    }
                    else
                    {
                        return BadRequest($"Erro ao remover cidades.");
                    }

                }
                else
                {
                    return BadRequest("Cidade não encontrada.");
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao remover cidades.{ex.Message}");
            }
        }





        [HttpGet("{id}")]
        public async Task<IActionResult> GetCidade([FromRoute] Guid id)
        {
            try
            {

                Cidade cidade = await _context.Cidade.FindAsync(id);
                if (cidade != null)
                {
                    return Ok(cidade);
                }
                else
                {
                    return BadRequest("Cidade não encontrada.");
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao consultar cidades.{ex.Message}");
            }
        }



        [HttpGet("Pesquisa")]
        public async Task<IActionResult> GetCidadePesquisa([FromQuery] string valor)
        {
            try
            {

                if (valor != null)
                {

                    //Usando o Expression
                    Expression<Func<Cidade, bool>> expressao = o => true;
                    expressao = o => o.Nome.ToUpper().Contains(valor.ToUpper())

                            || o.EstadoSigla.ToUpper().Contains(valor.ToUpper());

                    var lista = _context.Cidade.Where(expressao).ToList();


                    return Ok(lista);

                }
                else
                {
                    return BadRequest("Informe algum valor na pesquisa.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao consultar cidade.{ex.Message}");
            }
        }


        [HttpGet("Paginacao")]
        public async Task<IActionResult> GetCidadePaginacao([FromQuery] string valor, int skip, int take, bool ordemDesc)
        {
            try
            {
                // Valida os parâmetros
                var resultadoValidacao = ValidarParametrosPaginacao(valor, skip, take);
                if (!string.IsNullOrEmpty(resultadoValidacao))
                {
                    return BadRequest(resultadoValidacao);
                }

                // Usando o Query Criteria
                var lista = from o in _context.Cidade.ToList()
                            where o.EstadoSigla.ToUpper().Contains(valor.ToUpper())
                            || o.Nome.ToUpper().Contains(valor.ToUpper())
                            select o;

                if (ordemDesc)
                {
                    lista = from o in lista
                            orderby o.Nome descending
                            select o;
                }
                else
                {
                    lista = from o in lista
                            orderby o.Nome ascending
                            select o;
                }

                var qtde = lista.Count();

                lista = lista
                    .Skip(skip)
                    .Take(take)
                    .ToList();

                var paginacaoResponse = new PaginacaoResponse<Cidade>(lista, qtde, skip, take);

                return Ok(paginacaoResponse);

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao pesquisar cidades. {ex.Message}");
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

