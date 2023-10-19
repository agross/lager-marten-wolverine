using Marten;

using Microsoft.AspNetCore.Mvc;

namespace Api.UseCases.Mitarbeiter.NameÄndern;

public static class NameÄndernHandler
{
  public static  async Task<IResult> Handle(string id, [FromBody] string name, IDocumentSession session)
  {
    var maybeMitarbeiter = session.Load<Mitarbeiter>(id);
    if (maybeMitarbeiter == null)
    {
      return Results.Problem(new ProblemDetails { Status = 404 });
    }

    var kopie = maybeMitarbeiter with { Name = name };
    session.Store(kopie);
    await session.SaveChangesAsync();

    return Results.Ok();
  }
}
