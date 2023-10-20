using Api.UseCases.Planung.PlanAutomatischDeaktivieren;

using Microsoft.AspNetCore.Mvc;

using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;

namespace Api.UseCases.Planung.FürTagErstellen;

public static class FürTagErstellenEndpoint
{
  public record PlanCreationResponse(string Id)
    : CreationResponse("/plan/" + Id);

  [Tags("Plan")]
  [WolverinePost("/plan")]
  public static
    (PlanCreationResponse, IStartStream, ScheduledMessage<PlanDeaktivieren>)
    Handle(DateOnly tag)
  {
    // Validierung des Tags, z. B. nur in der Zukunft?

    // The following "ev" and "zeitpunktDerPlanDeaktivierung" instances could
    // also be created using a static Plan.Erfassen method.
    var key = $"plan/{tag.ToString("O")}";
    var ev = new PlanErstellt(tag);

    var zeitpunktDerPlanDeaktivierung = tag.ToDateTime(TimeOnly.MaxValue);
    // Just for debugging.
    zeitpunktDerPlanDeaktivierung = DateTime.Now.AddSeconds(10);

    return (new PlanCreationResponse(key),
            MartenOps.StartStream<Plan>(key, ev),
            new PlanDeaktivieren(key)
              .ScheduledAt(zeitpunktDerPlanDeaktivierung));
  }
}
