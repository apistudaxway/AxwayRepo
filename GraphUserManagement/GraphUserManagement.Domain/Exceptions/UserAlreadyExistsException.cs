namespace GraphUserManagement.Domain.Exceptions;

public class UserAlreadyExistsException : DomainException
{
    public UserAlreadyExistsException(string email) 
        : base($"User with email '{email}' already exists.") { }
    
    public UserAlreadyExistsException(string message, Exception innerException) 
        : base(message, innerException) { }
}