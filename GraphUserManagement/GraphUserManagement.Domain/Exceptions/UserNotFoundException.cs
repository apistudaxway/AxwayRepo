namespace GraphUserManagement.Domain.Exceptions;

public class UserNotFoundException : DomainException
{
    public UserNotFoundException(string userId) 
        : base($"User with ID '{userId}' was not found.") { }
    
    public UserNotFoundException(string message, Exception innerException) 
        : base(message, innerException) { }
}