using Marten;

namespace Api.UseCases.Mitarbeiter.Erfassen;

public static class ErfassenHandler
{
  public static async Task MitarbeiterErfassenHandler(Mitarbeiter ma, IDocumentSession session)
  {
    session.Store(ma);
    await session.SaveChangesAsync();
  }
}
