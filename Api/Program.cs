using Api.UseCases.Mitarbeiter;
using Api.UseCases.Mitarbeiter.Erfassen;
using Api.UseCases.Mitarbeiter.NameÄndern;
using Api.UseCases.Planung.AufgabeBeginnen;
using Api.UseCases.Planung.AufgabeErfassen;
using Api.UseCases.Planung.AufgabenAnzeigen;
using Api.UseCases.Planung.FürTagErstellen;

using Marten;
using Marten.Events;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;

using Oakton;

using Weasel.Core;

using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ApplyOaktonExtensions();

builder.Services.AddMarten(opt =>
       {
         opt.Connection(builder.Configuration.GetConnectionString("Marten")!);
         opt.AutoCreateSchemaObjects = AutoCreate.All;

         // opt.Schema.For<Mitarbeiter>().Duplicate(x => x.Vorname).Index(x => x.Vorname);

         opt.Events.StreamIdentity = StreamIdentity.AsString;

         opt.Projections.Add<PlanAufgabenProjection>(ProjectionLifecycle.Async);
         opt.Projections.Add<AufgabeZuPlanProjection>(ProjectionLifecycle.Async);
       })
       .IntegrateWithWolverine("wolverine_messages")
       .EventForwardingToWolverine()
       .ApplyAllDatabaseChangesOnStartup()
       .UseLightweightSessions()
       .AddAsyncDaemon(DaemonMode.Solo);

builder.Host.UseWolverine(o =>
{
  o.Policies.AutoApplyTransactions();
  o.Policies.UseDurableLocalQueues();
});

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapWolverineEndpoints();

app.MapPost("/plan/aufgabe",
            AufgabeErfassenHandler.Handle);

app.MapGet("/plan/{id}",
           AufgabenDesPlansAnzeigenHandler.Handle);

app.MapPost("/aufgabe/{aufgabenId}/beginnen",
            AufgabeBeginnenHandler.Handle);

await app.RunOaktonCommands(args);
