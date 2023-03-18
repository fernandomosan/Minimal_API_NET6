using ApiTarefas.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiTarefas.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<Tarefa> Tarefas => Set<Tarefa>();
    }
}
