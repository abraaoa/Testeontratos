using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Contratos.Models.Enums;
namespace Contratos.Models
{
    public class Prestacao
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required(ErrorMessage = "Data de vencimento da parcela obrigatória")]
        public DateTime DataVencimento { get; set; }

        public DateTime DataPagamento { get; set; }

        [Required(ErrorMessage = "Valor da prestação obrigatório")]
        [Range(1, long.MaxValue, ErrorMessage = "Valor da prestação deve ser maior que 0.")]
        public long Valor { get; set; }

        [NotMapped()]
        public string StatusPrestacao {
            get {
                StatusPrestacao enumStatus;
                if ( DataPagamento !=null && DataPagamento > DateTime.MinValue)
                {
                    enumStatus = Enums.StatusPrestacao.Baixada;
                }else
                {
                    if (DataVencimento >= DateTime.Now)
                    {
                        enumStatus = Enums.StatusPrestacao.Aberta;
                    }
                    else
                    {
                        enumStatus = Enums.StatusPrestacao.Atrasada;
                    }
                }

                return enumStatus.ToString(); 
            
            }
        }
 
        public long ContratoId { get; set; }

        public Contrato Contrato { get
                ; set; }

    }
}
