using System.Collections.Immutable;

namespace Api.UseCases.Planung;

public record PlanErstellt(DateOnly Tag);

public record AufgabeErfasst(Guid AufgabenId, string Aufgabe);

public record AufgabeBegonnen(Guid Id);

// Schreibseite.
public record Plan(string Id,
                   ImmutableHashSet<string> Aufgaben,
                   ImmutableHashSet<Guid> AufgabenIds,
                   ImmutableHashSet<Guid> BegonneneAufgaben
)
{
  public AufgabeErfasst ErfasseAufgabe(string aufgabe)
  {
    // Validierung.
    if (Aufgaben.Contains(aufgabe))
    {
      throw new ArgumentException($"Aufgabe {aufgabe} ist schon erfasst worden");
    }

    return new AufgabeErfasst(Guid.NewGuid(), aufgabe);
  }

  public AufgabeBegonnen AufgabeBeginnen(Guid id)
  {
    // Invarianten prüfen:

    // Aufgabe ist nicht erfasst.
    if (!AufgabenIds.Contains(id))
    {
      throw new ArgumentException($"Aufgabe {id} ist nicht erfasst worden");
    }

    // Schon begonnene Aufgaben werden abgelehnt.
    if (BegonneneAufgaben.Contains(id))
    {
      throw new ArgumentException($"Aufgabe {id} ist schon begonnen worden");
    }

    // Schon beendete Aufgaben werden abgelehnt.

    return new AufgabeBegonnen(id);
  }

  // Hydration:
  public static Plan Create(PlanErstellt ev)
    =>

      // Id sollte von Marten gesetzt werden.
      new("",
          ImmutableHashSet<string>.Empty,
          ImmutableHashSet<Guid>.Empty,
          ImmutableHashSet<Guid>.Empty);

  public Plan Apply(AufgabeErfasst ev)
    => this with
    {
      Aufgaben = Aufgaben.Add(ev.Aufgabe),
      AufgabenIds = AufgabenIds.Add(ev.AufgabenId),
    };

  public Plan Apply(AufgabeBegonnen ev)
    => this with
    {
      BegonneneAufgaben = BegonneneAufgaben.Add(ev.Id),
    };
}
