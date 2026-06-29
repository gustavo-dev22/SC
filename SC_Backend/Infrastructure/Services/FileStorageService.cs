using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Interfaces;

namespace Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _webRootPath;

        // 🚀 Recibe el string directo de la ruta física, eliminando el error de compilación por completo
        public FileStorageService(string webRootPath)
        {
            _webRootPath = webRootPath;
        }

        public async Task<string> GuardarArchivoAsync(Stream archivoStream, string nombreOriginal, string subCarpeta, CancellationToken cancellationToken)
        {
            if (archivoStream == null || archivoStream.Length == 0) return string.Empty;

            // Usamos la variable string que ya tiene la ruta mapeada
            string carpetaDestino = Path.Combine(_webRootPath, "uploads", subCarpeta);
            if (!Directory.Exists(carpetaDestino))
            {
                Directory.CreateDirectory(carpetaDestino);
            }

            string extension = Path.GetExtension(nombreOriginal);
            string nombreUnico = $"sustento_{Guid.NewGuid()}{extension}";
            string rutaCompletaFisica = Path.Combine(carpetaDestino, nombreUnico);

            using (var fileStream = new FileStream(rutaCompletaFisica, FileMode.Create))
            {
                await archivoStream.CopyToAsync(fileStream, cancellationToken);
            }

            return $"/uploads/{subCarpeta}/{nombreUnico}";
        }

        public void EliminarArchivoFisico(string rutaRelativa)
        {
            if (string.IsNullOrEmpty(rutaRelativa)) return;

            // Convertimos la URL relativa (/uploads/...) en un path físico absoluto del disco
            string rutaFisica = Path.Combine(_webRootPath, rutaRelativa.TrimStart('/'));

            if (File.Exists(rutaFisica))
            {
                File.Delete(rutaFisica);
            }
        }
    }
}
