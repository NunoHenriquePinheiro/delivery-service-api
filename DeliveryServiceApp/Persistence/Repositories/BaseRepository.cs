using DeliveryServiceApp.Persistence.Contexts;

namespace DeliveryServiceApp.Persistence.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly DataContext _context;

        protected BaseRepository(DataContext context)
        {
            _context = context;
        }
    }
}
