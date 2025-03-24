using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System.Threading.Tasks;

public class AuthService
{
    private readonly ApplicationDbContext _context;
    private readonly JwtService _jwtService;

    public AuthService(ApplicationDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<string?> Login(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        return _jwtService.GenerateToken(user);
    }

    public async Task<bool> CreateUser(string email, string password, bool isAdmin)
    {
        if (await _context.Users.AnyAsync(u => u.Email == email))
            return false;

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User
        {
            Email = email,
            PasswordHash = passwordHash,
            IsAdmin = isAdmin
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return true;
    }
}
