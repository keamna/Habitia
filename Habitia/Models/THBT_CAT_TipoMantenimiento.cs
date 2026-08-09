using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Habitia.Models.Catalogos
{
    [Table("THBT_CAT_TipoMantenimiento")]
    [Index(nameof(TC_Nombre), IsUnique = true)]
    public class TipoMantenimiento
    {
        [Key]
        public int TN_Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string TC_Nombre { get; set; }

        [Required]
        public bool TB_Estado { get; set; }

        // Relaciones
        public ICollection<Mantenimiento> Mantenimientos { get; set; } = new List<Mantenimiento>();
        public ICollection<PersonalTipoMantenimiento> PersonalAsignado { get; set; } = new List<PersonalTipoMantenimiento>();
    }
}