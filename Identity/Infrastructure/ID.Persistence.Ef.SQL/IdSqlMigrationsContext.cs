using ID.Infrastructure.Persistance.EF;
using Microsoft.EntityFrameworkCore;

namespace ID.Persistence.Ef.SQL;

public class IdSqlMigrationsContext(DbContextOptions<IdSqlMigrationsContext> options)
    : IdDbContext(options)
{ }