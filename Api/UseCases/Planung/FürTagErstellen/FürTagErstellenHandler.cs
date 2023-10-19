using Marten;
using Marten.Schema.Identity;

using Microsoft.AspNetCore.Mvc;

namespace Api.UseCases.Planung.FürTagErstellen;

public static class FürTagErstellenHandler
{
  public static async Task<IResult> Handle([FromBody] DateOnly tag, IDocumentSession session)
  {
    // Validierung vom Tag?

    var key = $"plan/{CombGuidIdGeneration.NewGuid()}";

    session.Events.StartStream<Plan>(key,
                                     new PlanErstellt(tag));

    await session.SaveChangesAsync();

    return Results.Created("/" + key, key);
  }
}
