using Habitia.Enums;

namespace Habitia.ViewModels.Vivienda
{
    public class ViviendaListaViewModel
    {
        // Identificador
        public int Id { get; set; }


        // Código o número de vivienda
        public string Numero { get; set; } = string.Empty;


        // Tipo de vivienda (Casa, Departamento, etc.)
        public TipoViviendaEnum Tipo { get; set; }


        // Estado (Activa / Inactiva)
        public EstadoViviendaEnum Estado { get; set; }


        // Cantidad máxima permitida
        public int CantidadInquilinos { get; set; }


        // Nombre completo del propietario
        public string? PropietarioNombre { get; set; }


        // Cantidad actual de inquilinos
        public int InquilinosActuales { get; set; }

        public bool ViveAhi { get; set; }
}
}