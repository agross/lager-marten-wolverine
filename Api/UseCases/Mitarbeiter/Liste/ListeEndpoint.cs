using Marten;

using Wolverine.Http;

namespace Api.UseCases.Mitarbeiter.Liste;

public static class ListeEndpoint
{
  [Tags("Mitarbeiter")]
  [WolverineGet("/mitarbeiter")]
  public static Task<IReadOnlyList<Mitarbeiter>> Handle(IQuerySession session)
    => session.Query<Mitarbeiter>().ToListAsync();
}
