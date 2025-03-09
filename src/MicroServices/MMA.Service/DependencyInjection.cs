/*
    This file create by Albie-Dev on 09/03/2025 3:45 PM
*/
using Microsoft.Extensions.DependencyInjection;

namespace MMA.Service
{
    /// <summary>
    /// This static class contains an extension method for adding MMA services to the 
    /// dependency injection container.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Extension method to register MMA services in the DI container.
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to.</param>
        /// <returns>The updated IServiceCollection with MMA services registered.</returns>
        public static IServiceCollection AddMMAService(this IServiceCollection services)
        {
            // Register MMA services here

            return services;
        }
    }
}
