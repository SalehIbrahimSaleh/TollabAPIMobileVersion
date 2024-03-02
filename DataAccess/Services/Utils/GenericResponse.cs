using System.Collections.Generic;

namespace DataAccess.Services.Utils
{
    public class GenericResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public IEnumerable<ValidationError> ValidationErrors { get; set; }
        public T Data { get; set; }
    }
}
