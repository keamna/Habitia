namespace Habitia.Helpers
{
    public static class ArchivoHelper
    {
        private static readonly string[] ExtensionesPermitidas = { ".jpg", ".jpeg", ".png" };
        private const long TamanoMaximoBytes = 5 * 1024 * 1024; // 5 MB

        public static async Task<string?> GuardarEvidenciaAsync(IFormFile? archivo, string webRootPath)
        {
            if (archivo == null || archivo.Length == 0)
                return null;

            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

            if (!ExtensionesPermitidas.Contains(extension))
                throw new InvalidOperationException("Formato de imagen no permitido. Use JPG o PNG.");

            if (archivo.Length > TamanoMaximoBytes)
                throw new InvalidOperationException("La imagen no puede superar los 5 MB.");

            var carpeta = Path.Combine(webRootPath, "uploads", "incidencias");
            Directory.CreateDirectory(carpeta);

            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            var rutaFisica = Path.Combine(carpeta, nombreArchivo);

            using (var stream = new FileStream(rutaFisica, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            // Ruta relativa que se guarda en BD y se usa en <img src="...">
            return $"/uploads/incidencias/{nombreArchivo}";
        }
        private static readonly string[] ExtensionesDocumento =
            { ".pdf", ".doc", ".docx", ".xls", ".xlsx" };

        private const long TamanoMaximoDocumento = 10 * 1024 * 1024; // 10 MB

        public static async Task<string> GuardarDocumentoAsync(
            IFormFile archivo, string webRootPath)
        {
            if (archivo == null || archivo.Length == 0)
                throw new InvalidOperationException("Debe seleccionar un archivo.");

            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

            if (!ExtensionesDocumento.Contains(extension))
                throw new InvalidOperationException(
                    "Formato no permitido. Use PDF, Word o Excel.");

            if (archivo.Length > TamanoMaximoDocumento)
                throw new InvalidOperationException(
                    "El documento no puede superar los 10 MB.");

            var carpeta = Path.Combine(webRootPath, "uploads", "documentos");
            Directory.CreateDirectory(carpeta);

            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            var rutaFisica = Path.Combine(carpeta, nombreArchivo);

            using (var stream = new FileStream(rutaFisica, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            // Ruta relativa que se guarda en BD
            return $"/uploads/documentos/{nombreArchivo}";
        }


        public static void EliminarArchivo(string? rutaRelativa, string webRootPath)
        {
            if (string.IsNullOrWhiteSpace(rutaRelativa))
                return;

            var rutaFisica = Path.Combine(
                webRootPath,
                rutaRelativa.TrimStart('/')
                            .Replace('/', Path.DirectorySeparatorChar));

            if (File.Exists(rutaFisica))
                File.Delete(rutaFisica);
        }
    }
}