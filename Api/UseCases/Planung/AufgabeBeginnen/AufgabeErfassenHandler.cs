using Marten;

using Microsoft.AspNetCore.Mvc;

namespace Api.UseCases.Planung.AufgabeBeginnen;

public static class AufgabeBeginnenHandler
{
  public static async Task<IResult> Handle(Guid aufgabenId, IDocumentSession session)
  {
    var mapping = session.Load<AufgabeZuPlan>(aufgabenId);

    var maybePlan = await session.Events.FetchForWriting<Plan>(mapping.PlanId);

    if (maybePlan.Aggregate == null)
    {
      return Results.NotFound();
    }

    var @event = maybePlan.Aggregate.AufgabeBeginnen(aufgabenId);
    maybePlan.AppendOne(@event);

    await session.SaveChangesAsync();

    return Results.Ok();
  }
}
