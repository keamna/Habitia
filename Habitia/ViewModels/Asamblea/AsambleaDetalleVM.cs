using Habitia.Models.Documentos;

namespace Habitia.ViewModels.Asamblea
{
    public class AsambleaDetalleVM
    {
        public Habitia.Models.Documentos.Asamblea Asamblea { get; set; }


        public List<ParticipanteAsamblea> Participantes { get; set; } = new();


        public List<Habitia.Models.Documentos.Documento> DocumentosAsociados { get; set; } = new();


        // Documentos que aún NO están asociados, para el selector
        public List<Habitia.Models.Documentos.Documento> DocumentosDisponibles { get; set; } = new();


        public int TotalConfirmados => Participantes.Count(p => p.TB_Confirmado);


        public int TotalAsistieron => Participantes.Count(p => p.TB_Asistio);


        // Solo se usa en la vista del residente
        public bool YoConfirme { get; set; }
    }
}