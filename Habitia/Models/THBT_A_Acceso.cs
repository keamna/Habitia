using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Habitia.Models;

[Table("THBT_A_Acceso")]
public class Acceso
{
    [Key]
    public int Id { get; set; }

    public int IdAutorizacion { get; set; }

    public DateTime FechaIngreso { get; set; }

    public DateTime? FechaSalida { get; set; }

    public bool Estado { get; set; }

    [ForeignKey(nameof(IdAutorizacion))]
    public Autorizacion Autorizacion { get; set; }
}