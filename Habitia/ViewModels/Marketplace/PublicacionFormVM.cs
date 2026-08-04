using System.ComponentModel.DataAnnotations;
using Habitia.Enums;

namespace Habitia.ViewModels.Marketplace
{
    public class PublicacionFormVM
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "El título es obligatorio")]
        [MaxLength(150)]
        public string Titulo { get; set; }


        [Required(ErrorMessage = "La descripción es obligatoria")]
        [MaxLength(1000)]
        public string Descripcion { get; set; }


        [Required(ErrorMessage = "Seleccione el tipo de publicación")]
        public TipoPublicacionEnum Tipo { get; set; } = TipoPublicacionEnum.Producto;


        [Required(ErrorMessage = "Seleccione una categoría")]
        public int IdCategoria { get; set; }


        [Range(0, 99999999, ErrorMessage = "El precio no es válido")]
        public decimal? Precio { get; set; }


        [MaxLength(1000)]
        public string? Especificaciones { get; set; }


        [DataType(DataType.Date)]
        public DateTime? FechaServicio { get; set; }


        public TimeSpan? HoraInicioServicio { get; set; }

        public TimeSpan? HoraFinServicio { get; set; }


        // ===== Campos del formulario con AM/PM =====

        public int? HoraInicioH { get; set; }
        public int? HoraInicioM { get; set; }
        public string? HoraInicioAmPm { get; set; } = "AM";

        public int? HoraFinH { get; set; }
        public int? HoraFinM { get; set; }
        public string? HoraFinAmPm { get; set; } = "AM";


        [Required(ErrorMessage = "El contacto es obligatorio")]
        [MaxLength(100)]
        public string Contacto { get; set; }

        public List<IFormFile>? Imagenes { get; set; }


        public List<Habitia.Models.ImagenPublicacion>? ImagenesExistentes { get; set; }



        // Convierte los selects (12h + AM/PM) a la hora real
        public void ArmarHoras()
        {
            HoraInicioServicio = Combinar(HoraInicioH, HoraInicioM, HoraInicioAmPm);
            HoraFinServicio = Combinar(HoraFinH, HoraFinM, HoraFinAmPm);
        }


        // Convierte la hora guardada a los selects (para Editar)
        public void DesarmarHoras()
        {
            var ini = Separar(HoraInicioServicio);
            HoraInicioH = ini.Item1;
            HoraInicioM = ini.Item2;
            HoraInicioAmPm = ini.Item3 ?? "AM";

            var fin = Separar(HoraFinServicio);
            HoraFinH = fin.Item1;
            HoraFinM = fin.Item2;
            HoraFinAmPm = fin.Item3 ?? "AM";
        }


        private static TimeSpan? Combinar(int? h, int? m, string? periodo)
        {
            if (!h.HasValue || !m.HasValue || string.IsNullOrWhiteSpace(periodo))
                return null;

            var hora = h.Value % 12;

            if (periodo.Equals("PM", StringComparison.OrdinalIgnoreCase))
                hora += 12;

            return new TimeSpan(hora, m.Value, 0);
        }


        private static Tuple<int?, int?, string?> Separar(TimeSpan? t)
        {
            if (!t.HasValue)
                return Tuple.Create<int?, int?, string?>(null, null, null);

            var periodo = t.Value.Hours >= 12 ? "PM" : "AM";

            var h = t.Value.Hours % 12;
            if (h == 0) h = 12;

            return Tuple.Create<int?, int?, string?>(h, t.Value.Minutes, periodo);
        }
    }
}