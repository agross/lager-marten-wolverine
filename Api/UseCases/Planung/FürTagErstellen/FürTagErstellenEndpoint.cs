using Microsoft.AspNetCore.Mvc;

using Wolverine.Http;
using Wolverine.Marten;

namespace Api.UseCases.Planung.FürTagErstellen;

public static class FürTagErstellenEndpoint
{
  public record PlanCreationResponse(string Id)
    : CreationResponse("/plan/" + Id);

  [Tags("Plan")]
  [WolverinePost("/plan")]
  public static (PlanCreationResponse, IStartStream) Handle([FromBody] DateOnly tag)
  {
    // Validierung vom Tag?

    var key = $"plan/{CombGuidIdGeneration.NewGuid()}";
    var ev = new PlanErstellt(tag);

    return (new PlanCreationResponse(key),
            MartenOps.StartStream<Plan>(key, ev));
  }
}
