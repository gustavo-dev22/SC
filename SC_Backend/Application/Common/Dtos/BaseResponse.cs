using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Dtos
{
    public class BaseResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        // Métodos estáticos de ayuda (Factory Methods) para un código más limpio
        public static BaseResponse<T> Ok(T data, string message = "Operación exitosa.") =>
            new() { Success = true, Message = message, Data = data };

        public static BaseResponse<T> Fail(string message, T? data = default) =>
            new() { Success = false, Message = message, Data = data };
    }
}
