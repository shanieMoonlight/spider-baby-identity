using ID.Infrastructure.Persistance.EF;
using Microsoft.EntityFrameworkCore;

namespace ID.Persistence.Ef.Postgres;

public class IdPostgresMigrationsContext(DbContextOptions<IdPostgresMigrationsContext> options)
    : IdDbContext(options)
{ }