using Marten;

using Microsoft.AspNetCore.Mvc;

namespace Api.UseCases.Planung.AufgabeErfassen;

public static class AufgabeErfassenHandler
{
  public record AufgabeErfassenRequest(string PlanId, string Aufgabe);

  public static async Task<IResult> Handle([FromBody] AufgabeErfassenRequest request, IDocumentSession session)
  {
    var maybePlan = await session.Events.FetchForWriting<Plan>(request.PlanId);

    if (maybePlan.Aggregate == null)
    {
      return Results.NotFound();
    }

    var @event = maybePlan.Aggregate.ErfasseAufgabe(request.Aufgabe);
    maybePlan.AppendOne(@event);

    await session.SaveChangesAsync();

    return Results.Ok();
  }
}
