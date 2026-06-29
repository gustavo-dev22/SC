using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> GuardarArchivoAsync(Stream archivoStream, string nombreOriginal, string subCarpeta, CancellationToken cancellationToken);
        void EliminarArchivoFisico(string rutaRelativa);
    }
}
