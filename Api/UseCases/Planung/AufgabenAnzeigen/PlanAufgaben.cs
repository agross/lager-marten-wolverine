using System.Collections.Immutable;

using Marten.Events;
using Marten.Events.Aggregation;

namespace Api.UseCases.Planung.AufgabenAnzeigen;

// Leseseite.
public record PlanAufgaben(string Id,
                           ImmutableDictionary<Guid,string> Aufgaben);

public class PlanAufgabenProjection : SingleStreamProjection<PlanAufgaben>
{
  public static PlanAufgaben Create(IEvent<PlanErstellt> ev)
  {
    return new PlanAufgaben(ev.StreamKey!, ImmutableDictionary.Create<Guid, string>());
  }

  public PlanAufgaben Apply(AufgabeErfasst ev, PlanAufgaben current)
  {
    return current with
    {
      Aufgaben = current.Aufgaben.Add(ev.AufgabenId, ev.Aufgabe)
    };
  }
}
