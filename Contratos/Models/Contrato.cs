using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Contratos.Models
{
    public class Contrato
    {
        [Key]
        [ReadOnly(true)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required(ErrorMessage = "Data de contratação obrigatória")]
        public DateTime DataContratacao { get; set; }

        [Required(ErrorMessage = "Quantidade de parcelas obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade de parcelas deve ser maior que 0.")]
        public int QuantidadeParcelas { get; set; }

        [Required(ErrorMessage = "Valor financiado obrigatório")]
        [Range(1, long.MaxValue, ErrorMessage = "Valor financiado deve ser maior que 0.")]
        public long ValorFianciado { get; set; }

        public ICollection<Prestacao> Prestacoes { get; set; }


    }


}
