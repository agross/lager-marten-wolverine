using Marten.Schema;

using Wolverine.Marten;

namespace Api.UseCases.Planung.PlanAutomatischDeaktivieren;

// IdentityAttribute is used to the AggregateHandler below knows how to find
// the Plan we want to deactivate.
public record PlanDeaktivieren([property: Identity] string PlanId);

public class PlanAutomatischDeaktivierenHandler
{
  // EventForwardingToWolverine() in Program.cs will forward all events appended
  // to streams to Wolverine. Subscribe to the PlanErstellt events and schedule
  // a message for the end of the day the plan was created to e.g. deactivate
  // the plan. Scheduled messages are durable in the database Wolverine uses
  // (here: the same DB that Marten uses, wolverine_incoming_envelopes table).

  // The issue with the PlanErstellt is that it only contains the day, but not the
  // plan ID (primary key) - one option is to extend PlanErstellt to also include
  // the plan ID, or to schedule the deactivation of the plan in the Wolverine
  // HTTP handler that creates plans (which is what I did - see FürTagErstellenEndpoint.

  // public static ScheduledMessage<PlanDeaktivieren> Handle(PlanErstellt ev)
  //   => new PlanDeaktivieren(???)
  //     .ScheduledAt(ev.Tag.ToDateTime(TimeOnly.MaxValue));

  // ---

  // This gets called before Handle below.
  // public static Task<IEventStream<Plan>> LoadAsync(PlanDeaktivieren command, IDocumentSession session)
  //   => session.Events.FetchForWriting<Plan>(command.PlanId);

  // This is the actual command handling code.
  // public static void Handle(PlanDeaktivieren command,
  //                           IEventStream<Plan> plan,
  //                           ILogger<PlanAutomatischDeaktivierenHandler> logger)
  // {
  //   logger.LogInformation("Plan wird deaktiviert: {0}", plan.Aggregate.Id);
  //
  //   var ev = plan.Aggregate.Deaktivieren();
  //
  //   plan.AppendOne(ev);
  // }

  // Since the "load aggregate" (LoadAsync) and "handle command" (Handle) pattern will be used many
  // times, there's a functionally equivalent short version.
  // https://wolverine.netlify.app/guide/durability/marten/event-sourcing.html#handler-method-signatures

  [AggregateHandler]
  public static Events Handle(PlanDeaktivieren command,
                              Plan plan,
                              ILogger<PlanAutomatischDeaktivierenHandler> logger)
  {
    logger.LogInformation("Plan wird deaktiviert: {0}", plan.Id);

    var ev = plan.Deaktivieren();

    // The events that will be appended to IEventStream<Plan>
    // We could also return the event itself. If next to the event, other
    // message(s) needs to be sent, I find it more explicit to use Events for
    // "append these events to the aggregate stream" and OutgoingMessages for
    // "this is supposed to be routed to other handlers". OutgoingMessages can
    // also be scheduled for a later time.
    return new Events { ev };
  }
}
