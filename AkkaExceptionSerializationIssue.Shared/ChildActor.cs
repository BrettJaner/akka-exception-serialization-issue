using Akka.Actor;
using Akka.Event;

namespace AkkaExceptionSerializationIssue.Shared
{
    public class ChildActor : ReceiveActor
    {
        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new ChildActor());
        }

        public ChildActor()
        {
            Receive<Messages.ThrowException>(OnReceiveThrowException);
            Receive<Messages.LogMessage>(OnReceiveLogMessage);
        }

        private void OnReceiveThrowException(Messages.ThrowException msg)
        {
            throw new InvalidOperationException("You can't do this.");
        }

        private void OnReceiveLogMessage(Messages.LogMessage msg)
        {
            Context.GetLogger().Info("You can do this.");
        }

        public class Messages
        {
            public sealed class ThrowException
            {
            }

            public sealed class LogMessage
            {
            }
        }
    }
}