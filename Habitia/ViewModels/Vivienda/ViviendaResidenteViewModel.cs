namespace Habitia.ViewModels.Financiero.Admin
{
    // Representa un vínculo activo vivienda-residente (THBT_A_ViviendaUsuario),
    // usado en la tabla de selección masiva al crear un cargo.
    public class ViviendaResidenteViewModel
    {
        public int TN_IdViviendaUsuario { get; set; } // id del vínculo, valor del checkbox
        public int TN_IdVivienda { get; set; }
        public string NumeroVivienda { get; set; }
        public string TC_IdResidente { get; set; }
        public string NombreResidente { get; set; }
        public string IdentificacionResidente { get; set; }
        public string TipoRelacion { get; set; } // "Propietario" / "Inquilino"
    }

    // Vivienda simple, usada por el selector dinámico cuando un residente
    // (modo individual) tiene más de una vivienda asociada.
    public class ViviendaSimpleViewModel
    {
        public int TN_Id { get; set; }
        public string TC_Numero { get; set; }
    }
}