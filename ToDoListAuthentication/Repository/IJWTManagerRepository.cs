using ToDoListAuthentication.Controllers;
using ToDoListAuthentication.Models;

namespace ToDoListAuthentication.Repository
{
    public interface IJWTManagerRepository
    {
        Tokens Authenticate(Users users);
    }
}
