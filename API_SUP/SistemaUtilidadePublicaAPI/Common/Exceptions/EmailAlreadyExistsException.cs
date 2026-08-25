namespace SistemaUtilidadePublicaAPI.Common.Exceptions
{
    public class EmailAlreadyExistsException: Exception
    {
        public EmailAlreadyExistsException() 
            : base("Este email já está registrado!")
        {
                    
        }
    }
}
