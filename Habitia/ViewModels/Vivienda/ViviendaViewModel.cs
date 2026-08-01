// Usado por el Admin para Crear/Editar.  NO incluye CantidadInquilinos:
// ese dato solo lo gestiona el propietario (residente) desde su panel.
using System.ComponentModel.DataAnnotations;
using Habitia.Enums;

namespace Habitia.ViewModels.Vivienda
{
    public class ViviendaViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe completar todos los campos obligatorios.")]
        public string Numero { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe completar todos los campos obligatorios.")]
        public TipoViviendaEnum Tipo { get; set; }
    }
}