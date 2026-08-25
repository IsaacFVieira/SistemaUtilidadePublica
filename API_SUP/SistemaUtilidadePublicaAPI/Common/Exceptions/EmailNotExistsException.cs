namespace SistemaUtilidadePublicaAPI.Common.Exceptions
{
    public class EmailNotExistsException : Exception
    {

        public EmailNotExistsException()
            :base("Este Email não está registado!") {
        
        }
    }
}
