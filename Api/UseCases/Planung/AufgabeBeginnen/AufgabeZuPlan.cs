using Marten.Events;
using Marten.Events.Projections;

namespace Api.UseCases.Planung.AufgabeBeginnen;

public record AufgabeZuPlan(Guid Id, string PlanId);

public class AufgabeZuPlanProjection : MultiStreamProjection<AufgabeZuPlan, Guid>
{
  public AufgabeZuPlanProjection()
  {
    Identity<AufgabeErfasst>(x => x.AufgabenId);
  }

  public static AufgabeZuPlan Create(IEvent<AufgabeErfasst> ev)
  {
    return new AufgabeZuPlan(ev.Data.AufgabenId, ev.StreamKey!);
  }
}
