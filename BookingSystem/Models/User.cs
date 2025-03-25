using System.ComponentModel.DataAnnotations;

namespace BookingSystem.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public List<Booking> Bookings { get; set; } = new();
}