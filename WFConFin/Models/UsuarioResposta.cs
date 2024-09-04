using System.ComponentModel.DataAnnotations;

namespace WFConFin.Models
{
    public class UsuarioResposta
    {
        public Usuario Usuario { get; set; }
        public string Token { get; set; }

    }
}
