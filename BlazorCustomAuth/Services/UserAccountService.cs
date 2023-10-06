using BlazorCustomAuth.Models;

namespace BlazorCustomAuth.Services;

public class UserAccountService
{
    private List<User> users = new List<User>();

    public UserAccountService()
    {
        users = new List<User>
            {
                new User(){Username ="admin", Password="admin", Role="Administrator"},
                new User(){Username ="user", Password="user", Role="User"},
            };
    }

    public User? GetUserByName(string userName)
    {
        return users.FirstOrDefault(x => x.Username == userName);
    }
}
