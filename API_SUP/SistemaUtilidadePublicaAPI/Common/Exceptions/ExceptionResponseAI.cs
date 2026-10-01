using SistemaUtilidadePublicaAPI.Models;

namespace SistemaUtilidadePublicaAPI.Common.Exceptions
{
    public class ExceptionResponseAI : Exception
    {
        public ExceptionResponseAI(String message) : base(message.ToString())
          { }
     }
}
