using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WFConFin.Models
{
    public class Cidade
    {

        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo nome deve ser informado.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "O campo nome deve ter entre 3 e 200 caracteres.")]
        public string Nome { get; set; }


        [Required(ErrorMessage = "O camo estado é obrigatório")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "O campo estado deve ter 2 caracteres.")]
        public string EstadoSigla { get; set; }


        //Criar o Id para a cidade
        public Cidade()
        {
            Id = Guid.NewGuid();
        }

        //Realacionamento Entity Framework
        [JsonIgnore]
        public Estado Estado { get; set; }

    }
}
