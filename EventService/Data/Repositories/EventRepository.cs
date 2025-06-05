using Data.Contexts;
using Data.Entities;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data.Repositories;

public class EventRepository(DataContext context) : BaseRepository<EventEntity>(context), IEventRepository
{
    public override async Task<RepositoryResult<IEnumerable<EventEntity>>> GetAllAsync()
    {
        try
        {
            var entities = await _table
                .Include(x => x.EventsPackages)
                .ThenInclude(ep => ep.Package)
                .ToListAsync();
            return new RepositoryResult<IEnumerable<EventEntity>> { Success = true, Result = entities };

        }
        catch (Exception ex)
        {
            return new RepositoryResult<IEnumerable<EventEntity>>
            {
                Success = false,
                Error = ex.Message,

            };
        }
    }

    public override async Task<RepositoryResult<EventEntity?>> GetAsync(Expression<Func<EventEntity, bool>> expression)
    {
        try
        {
            var entity = await _table
                .Include(x => x.EventsPackages)
                .ThenInclude(ep => ep.Package)
                .FirstOrDefaultAsync(expression)
                 ?? throw new Exception("Not Found");
            
            return new RepositoryResult<EventEntity?> { Success = true, Result = entity };

        }
        catch (Exception ex)
        {
            return new RepositoryResult<EventEntity?>
            {
                Success = false,
                Error = ex.Message,

            };
        }
    }

}
