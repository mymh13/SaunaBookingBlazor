public class Booking
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public BookingStatus Status { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime? ReservationExpires { get; set; }
}

public enum BookingStatus
{
    Available,
    Reserved,
    Booked,
    Disabled
}