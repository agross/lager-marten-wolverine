using Marten;

using Wolverine.Http;

namespace Api.UseCases.Mitarbeiter.Erfassen;

public static class ErfassenEndpoint
{
  // Introducing this special type just for the http response
  // gives us back the 201 status code
  public record MitarbeiberCreationResponse(string Id)
    : CreationResponse("/mitarbeiter/" + Id);

  [Tags("Mitarbeiter")]
  [WolverinePost("/mitarbeiter")]
  public static MitarbeiberCreationResponse Handle(Mitarbeiter ma, IDocumentSession session)
  {
    // Store Mitarbeiter as-is.
    session.Store(ma);

    // By Wolverine.Http conventions, the first "return value" is always
    // assumed to be the Http response, and any subsequent values are
    // handled independently
    return new MitarbeiberCreationResponse(ma.Id);
  }
}
