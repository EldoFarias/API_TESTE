using System.ComponentModel.DataAnnotations;

namespace WFConFin.Models
{
    public class UsuarioLogin
    {

        [Required(ErrorMessage = "O campo login é obrigatório.")]
        [StringLength(45, MinimumLength = 3, ErrorMessage = "O campo login deve ter entre 3 e 45 caracteres")]
        public string Login { get; set; }

        [Required(ErrorMessage = "O campo password é obrigatório.")]
        [StringLength(45, MinimumLength = 3, ErrorMessage = "O campo password deve ter entre 3 e 45 caracteres")]
        public string Password { get; set; }


    }
}
