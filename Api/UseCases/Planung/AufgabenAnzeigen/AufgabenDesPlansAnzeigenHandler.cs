using Marten;

using Microsoft.AspNetCore.Mvc;

namespace Api.UseCases.Planung.AufgabenAnzeigen;

public static class AufgabenDesPlansAnzeigenHandler
{
  public static IResult Handle([FromQuery]string id, IDocumentSession session)
  {
    var mayBePlanAufgaben = session.Load<PlanAufgaben>(id);

    return Results.Json(mayBePlanAufgaben);
  }
}
