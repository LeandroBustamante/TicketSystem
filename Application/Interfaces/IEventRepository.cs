using Domain.Entities;

namespace Application.Interfaces;

// Definimos las operaciones que podemos hacer con los Eventos y Sectores
public interface IEventRepository
{
    // Obtiene la lista de todos los eventos para el catálogo 
    Task<IEnumerable<Event>> GetAllAsync();

    // Obtiene un evento específico por su ID
    Task<Event?> GetByIdAsync(int id);

    // Obtiene los sectores asociados a un evento específico 
    Task<IEnumerable<Sector>> GetSectorsByEventIdAsync(int eventId);
}