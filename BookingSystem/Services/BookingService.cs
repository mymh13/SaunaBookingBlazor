using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookingSystem.Models;
using BookingSystem.Data;

namespace BookingSystem.Services
{
    public class BookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BookingService> _logger;

        public BookingService(ApplicationDbContext context, ILogger<BookingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Booking>> GetWeeklyBookings(DateTime startDate)
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Where(b => b.StartTime >= startDate && b.StartTime < startDate.AddDays(7))
                .ToListAsync();
        }

        public async Task<bool> ReserveBooking(int userId, DateTime startTime)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.StartTime == startTime && b.Status == BookingStatus.Available);

            if (booking == null)
                return false;

            booking.Status = BookingStatus.Reserved;
            booking.UserId = userId;
            booking.ReservationExpires = DateTime.UtcNow.AddMinutes(5);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ConfirmBooking(int userId, int bookingId)
        {
            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId && b.Status == BookingStatus.Reserved);

            if (booking == null || booking.ReservationExpires < DateTime.UtcNow)
                return false;

            booking.Status = BookingStatus.Booked;
            booking.ReservationExpires = null;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}