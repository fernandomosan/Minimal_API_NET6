using ApiTarefas.Data;
using ApiTarefas.Models;
using Carter;
using Microsoft.EntityFrameworkCore;

namespace ApiTarefas.Services.CarterEndPoints
{
    public class TarefasModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            MethodsGet(app);
            MethodsPost(app);
            MethodsPut(app);
            MethodsDelete(app);
        }

        public void MethodsGet(IEndpointRouteBuilder app)
        {
            app.MapGet("/", () => "Olá Mundo");

            app.MapGet("frases", async () =>
                await new HttpClient().GetStringAsync("https://random-word-api.herokuapp.com/word?lang=it")
            );

            app.MapGet("/tarefas", async (AppDbContext db) => await db.Tarefas.ToListAsync());

            app.MapGet("/tarefas/{id}", async (int id, AppDbContext db) =>
                                        await db.Tarefas.FindAsync(id) is Tarefa tarefa ? Results.Ok(tarefa) : Results.NotFound());

            app.MapGet("/tarefas/concluidas", async (AppDbContext db) =>
                                              await db.Tarefas.Where(t => t.IsConcluida).ToListAsync());
        }

        public void MethodsPost(IEndpointRouteBuilder app)
        {
            app.MapPost("/tarefas", async (Tarefa tarefa, AppDbContext db) =>
            {
                db.Tarefas.Add(tarefa);
                await db.SaveChangesAsync();
                return Results.Created($"/tarefas/{tarefa.Id}", tarefa);
            });
        }

        public void MethodsPut(IEndpointRouteBuilder app)
        {
            app.MapPut("/tarefas/{id}", async (int id, Tarefa inputTarefa, AppDbContext db) =>
            {
                if (inputTarefa.Id != id) return Results.NotFound();

                var tarefa = await db.Tarefas.FindAsync(id);
                if (tarefa is null) return Results.NotFound();

                tarefa.Nome = inputTarefa.Nome;
                tarefa.IsConcluida = inputTarefa.IsConcluida;

                await db.SaveChangesAsync();
                return Results.NoContent();
            });
        }

        public void MethodsDelete(IEndpointRouteBuilder app)
        {
            app.MapDelete("/tarefas/{id}", async (int id, AppDbContext db) =>
            {
                if (await db.Tarefas.FindAsync(id) is Tarefa tarefa)
                {
                    db.Tarefas.Remove(tarefa);
                    await db.SaveChangesAsync();
                    return Results.Ok(tarefa);
                }

                return Results.NotFound();
            });
        }
    }
}
