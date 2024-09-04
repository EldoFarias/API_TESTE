using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFConFin.Models
{
    public class Pessoa
    {

        [Key]
        public  Guid Id { get; set; }


        [Required(ErrorMessage = "O campo nome é obrigatório.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "O campo nome deve ter entre 3 e 200 caracteres")]
        public string Nome { get; set; }

        [StringLength(20, ErrorMessage = "O campo telefone deve ter até 20 caracteres")]
        public string Telefone { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DataNascimento { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal Salario { get; set; }


        [StringLength(20, ErrorMessage = "O campo genero deve ter até 20 caracteres.")]
        [Column("Genero")]
        public string Genero { get; set; }


        public Guid? CidadeId { get; set; }


        public Pessoa()
        {
            Id = Guid.NewGuid();
        }

        //relacionamento Entity Framework
        public Cidade Cidade { get; set; }





    }
}
