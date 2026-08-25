namespace SistemaUtilidadePublicaAPI.Common.Exceptions
{
    public class InvalidPasswordException: Exception
    {
        public InvalidPasswordException()
            : base("PassWord Incorreta!")
        { }
    }
}
