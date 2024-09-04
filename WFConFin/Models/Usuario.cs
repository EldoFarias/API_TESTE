using System.ComponentModel.DataAnnotations;

namespace WFConFin.Models
{
    public class Usuario
    {

        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo nome é obrigatório.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "O campo nome deve ter entre 3 e 200 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo login é obrigatório.")]
        [StringLength(45, MinimumLength = 3, ErrorMessage = "O campo login deve ter entre 3 e 45 caracteres")]
        public string Login { get; set; }

        [Required(ErrorMessage = "O campo password é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O campo password deve ter entre 3 e 30 caracteres")]
        public string Password { get; set; }

        [Required(ErrorMessage = "O campo funçao é obrigatório.")]
        [StringLength(45, MinimumLength = 3, ErrorMessage = "O campo funçao deve ter entre 3 e 45 caracteres")]
        public string Funcao { get; set; }



    }
}
