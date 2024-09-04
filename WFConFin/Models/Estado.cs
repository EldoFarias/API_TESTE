using System.ComponentModel.DataAnnotations;

namespace WFConFin.Models
{
    public class Estado
    {

        [Key]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "O campo dever ter 2 caracteres.")]
        public string Sigla { get; set; }


        [Required(ErrorMessage = "O campo nome é obrigatório.")]
        [StringLength(60, MinimumLength = 3, ErrorMessage = "O campo nome deve ter entre 3 e caracteres.")]
        public string Nome { get; set; }


    }
}
