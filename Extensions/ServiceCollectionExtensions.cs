using Microsoft.Extensions.DependencyInjection;
using Travely.Services.Bookings;

namespace Travely.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBookingModule(this IServiceCollection services)
        {
            services.AddScoped<IBookingService, BookingService>();
            return services;
        }
    }
}