using Akka.Actor;

namespace AkkaExceptionSerializationIssue.Shared
{
    public class ParentActor : ReceiveActor
    {
        public static Props Props()
        {
            return Akka.Actor.Props.Create(() => new ParentActor());
        }

        public ParentActor()
        {
            Receive<Messages.RegisterChild>(OnReceiveRegisterChild);
            Receive<Messages.ThrowExceptionOnChild>(OnReceiveThrowExceptionOnChild);
            Receive<Messages.LogMessageOnChild>(OnReceiveLogMessageOnChild);
        }

        private void OnReceiveRegisterChild(Messages.RegisterChild msg)
        {
            if (string.IsNullOrWhiteSpace(msg.ChildName))
            {
                throw new ArgumentException($"{nameof(msg.ChildName)} can not be null, empty, or whitespace.");
            }

            var existingDevice = Context.Child(msg.ChildName);

            if (!existingDevice.Equals(ActorRefs.Nobody))
            {
                Context.Stop(existingDevice);
            }

            Context.ActorOf(
                ChildActor.Props().WithDeploy(Deploy.None.WithScope(new RemoteScope(Context.Sender.Path.Address))),
                msg.ChildName);
        }

        private void OnReceiveThrowExceptionOnChild(Messages.ThrowExceptionOnChild msg)
        {
            if (string.IsNullOrWhiteSpace(msg.ChildName))
            {
                throw new ArgumentException($"{nameof(msg.ChildName)} can not be null, empty, or whitespace.");
            }

            var deviceActor = Context.Child(msg.ChildName);

            deviceActor.Tell(new ChildActor.Messages.ThrowException());
        }

        private void OnReceiveLogMessageOnChild(Messages.LogMessageOnChild msg)
        {
            if (string.IsNullOrWhiteSpace(msg.ChildName))
            {
                throw new ArgumentException($"{nameof(msg.ChildName)} can not be null, empty, or whitespace.");
            }

            var deviceActor = Context.Child(msg.ChildName);

            deviceActor.Tell(new ChildActor.Messages.LogMessage());
        }

        public class Messages
        {
            public sealed class RegisterChild
            {
                public string? ChildName { get; set; }
            }

            public sealed class ThrowExceptionOnChild
            {
                public string? ChildName { get; set; }
            }

            public sealed class LogMessageOnChild
            {
                public string? ChildName { get; set; }
            }
        }
    }
}