using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using Newtonsoft.Json;

namespace AngularForms.Model
{
    [Table("FUNCIONAMENTO_ESTABELECIMENTO")]
    public class FuncionamentoEstabelecimento
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("DIA_SEMANA", Order=1)]
        public int DiaSemana { get; set; }

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Column("ABERTURA", Order = 2)]
        public string Abertura { get; set; }

        [Required]
        [Column("FECHAMENTO")]
        public string Fechamento { get; set; }

        [Required]
        [Column("TEM_DELIVERY")]
        public bool TemDelivery { get; set; }
    }
}