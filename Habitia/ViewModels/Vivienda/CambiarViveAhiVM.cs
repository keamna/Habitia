namespace Habitia.ViewModels.Vivienda
{
    // Se necesitan ambos IDs porque, a diferencia del residente (que solo
    // puede tocar su propia relación), el Admin puede cambiar "vive ahí"
    // de cualquier usuario asociado a la vivienda.
    public class CambiarViveAhiVM
    {
        public int IdVivienda { get; set; }
        public string IdUsuario { get; set; } = string.Empty;
    }
}