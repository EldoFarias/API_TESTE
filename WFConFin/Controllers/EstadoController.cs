using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WFConFin.Dados;
using WFConFin.Models;

namespace WFConFin.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class EstadoController : Controller
    {

        private readonly WFConfinDBContext _context;

        public EstadoController(WFConfinDBContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> GetEstados()
        {
            try
            {
                var result = _context.Estado.ToList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro na listagem dos estados.{ex.Message}");
            }
        }


        [HttpPost]
        public async Task<IActionResult> PostEstado([FromBody] Estado estado)
        {
            try
            {

                await _context.Estado.AddAsync(estado);
                var valor = await _context.SaveChangesAsync();

                if (valor == 1)
                {
                    return Ok("Sucesso ao inserir estado.");
                }
                else
                {
                    return BadRequest("Estado não incluido.");
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao criar estado.{ex.Message}");
            }
        }

        [HttpPost("batch")]
        public async Task<IActionResult> PostEstados([FromBody] List<Estado> estados)
        {
            try
            {
                if (estados == null || estados.Count == 0)
                {
                    return BadRequest("A lista de estados está vazia.");
                }

                await _context.Estado.AddRangeAsync(estados);
                var valor = await _context.SaveChangesAsync();

                if (valor == estados.Count)
                {
                    return Ok("Sucesso ao inserir todos os estados.");
                }
                else
                {
                    return BadRequest("Nem todos os estados foram incluídos.");
                }
            }
            catch (Exception ex)
            {
                // Exibe a mensagem da InnerException para obter mais detalhes
                return BadRequest($"Erro ao criar estados. {ex.Message} Inner Exception: {ex.InnerException?.Message}");
            }
        }



        [HttpPut]
        public async Task<IActionResult> PutEstado([FromBody] Estado estado)
        {
            try
            {

                _context.Estado.Update(estado);
                var valor = await _context.SaveChangesAsync();

                if (valor == 1)
                {
                    return Ok("Sucesso ao atualizar estado.");
                }
                else
                {
                    return BadRequest("Estado não atualizar.");
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar estado.{ex.Message}");
            }
        }


        [HttpDelete("{sigla}")]
        public async Task<IActionResult> DeleteEstado([FromRoute] string sigla)
        {
            try
            {

                var estado = await _context.Estado.FindAsync(sigla);

                if (estado.Sigla == sigla && !string.IsNullOrEmpty(estado.Sigla))
                {
                    _context.Estado.Remove(estado);
                    var valor = await _context.SaveChangesAsync();

                    if (valor == 1)
                    {
                        return Ok("Estado excluído com sucesso.");
                    }
                    else
                    {
                        return BadRequest("Erro ao excluir estado.");
                    }
                }
                else
                {
                    return NotFound("Erro, estado não existe.");
                }


            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao atualizar estado.{ex.Message}");
            }
        }


        [HttpGet("{sigla}")]
        public async Task<IActionResult> GetEstado([FromRoute] string sigla)
        {
            try
            {

                var estado = await _context.Estado.FindAsync(sigla);

                if (estado != null)
                {
                    if (estado.Sigla == sigla && !string.IsNullOrEmpty(estado.Sigla))
                    {
                        return Ok(estado);
                    }
                    else
                    {
                        return NotFound("Erro, estado não existe.");
                    }
                }
                else
                {
                    return NotFound("Erro, estado não existe.");
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao consultar estado.{ex.Message}");
            }
        }


        [HttpGet("Pesquisa")]
        public async Task<IActionResult> GetEstadoPesquisa([FromQuery] string valor)
        {
            try
            {
                // Usando o Query Criteria
                /* var lista = from o in _context.Estado.ToList()
                             where o.Sigla.ToUpper().Contains(valor.ToUpper())
                             || o.Nome.ToUpper().Contains(valor.ToUpper())
                             select o;

                 */

                //Usando o Entity
                /*lista =  _context.Estado

                          .Where(o => o.Sigla.ToUpper().Contains(valor.ToUpper())

                          || o.Nome.ToUpper().Contains(valor.ToUpper())

                           ).ToList();

                  */

                if (valor != null)
                {

                    //Usando o Expression
                    Expression<Func<Estado, bool>> expressao = o => true;
                    expressao = o => o.Sigla.ToUpper().Contains(valor.ToUpper())

                            || o.Nome.ToUpper().Contains(valor.ToUpper());

                    var lista = await _context.Estado.Where(expressao).ToListAsync();


                    return Ok(lista);
                }
                else
                {
                    return BadRequest("Informe algum valor na pesquisa.");
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao consultar estado.{ex.Message}");
            }
        }

        [HttpGet("Paginacao")]
        public async Task<IActionResult> GetEstadoPaginacao([FromQuery] string valor, int skip, int take, bool ordemDesc)
        {
            try
            {
                var lista = from o in _context.Estado.ToList()
                            select o;   


                // Valida os parâmetros
                var resultadoValidacao = ValidarParametrosPaginacao(skip, take);
                if (!string.IsNullOrEmpty(resultadoValidacao))
                {
                    return BadRequest(resultadoValidacao);
                }


                if (!String.IsNullOrEmpty(valor))
                {
                    lista = from o in _context.Estado.ToList()
                            where o.Sigla.ToUpper().Contains(valor.ToUpper())
                            || o.Nome.ToUpper().Contains(valor.ToUpper())
                            select o;
                }              

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
                    .Skip((skip - 1 ) * take)
                    .Take(take)
                    .ToList();

                var paginacaoResponse = new PaginacaoResponse<Estado>(lista, qtde, skip, take);


                return Ok(paginacaoResponse);

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao pesquisar estados.{ex.Message}");
            }
        }



        private string ValidarParametrosPaginacao( int skip, int take)
        {
          /*  if (string.IsNullOrEmpty(valor))
            {
                return "O parâmetro 'valor' é obrigatório e não pode estar vazio.";
            }
          */

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
