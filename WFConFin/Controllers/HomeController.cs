using Microsoft.AspNetCore.Mvc;
using WFConFin.Models;

namespace WFConFin.Controllers
{

    [ApiController]
    [Route("[controller]")]

    public class HomeController : Controller
    {

        private static List<Estado> listaEstados = new List<Estado>();

        [HttpGet("estado")]
        public IActionResult GetEstados()
        {

            return Ok(listaEstados);

        }

        [HttpPost("estado")]
        public  IActionResult PostEstados([FromBody] Estado estado)
        {

            listaEstados.Add(estado);
            return Ok("Estado cadastrado com sucesso.");

        }

        [HttpGet]
        public async Task<IActionResult> GetInformacao()
        {

            var result = "Retorno em texto";
            return Ok(result);
            
        }


        [HttpGet("info2")]
        public async Task<IActionResult> GetInformacao2()
        {

            var result = "Retorno em texto 2";
            return Ok(result);


        }


        [HttpGet("info3/{valor}")]
        public async Task<IActionResult> GetInformacao3([FromRoute] string valor)
        {

            var result = "Retorno em texto 3  - valor: " + valor;
            return Ok(result);


        }

        [HttpPost("info4")]
        public async Task<IActionResult> GetInformacao4(string valor)
        {

            var result = "Retorno em texto 4  - valor: " + valor;
            return Ok(result);

        }

        [HttpGet("info5")]
        public async Task<IActionResult> GetInformacao5([FromHeader] string valor)
        {

            var result = "Retorno em texto 5  - valor: " + valor;
            return Ok(result);

        }

        [HttpPost("info6")]
        public async Task<IActionResult> GetInformacao6([FromBody] Corpo corpo)
        {

            var result = "Retorno em texto 6  - valor: " + corpo.valor;
            return Ok(result);

        }

    }


    public class Corpo
    {
        public string valor { get; set; }
    }


}
