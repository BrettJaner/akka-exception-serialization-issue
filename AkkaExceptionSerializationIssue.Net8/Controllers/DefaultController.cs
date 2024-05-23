using Akka.Actor;
using Akka.Hosting;
using AkkaExceptionSerializationIssue.Shared;
using Microsoft.AspNetCore.Mvc;

namespace AkkaExceptionSerializationIssue.Net8.Controllers
{
    [ApiController]
    [Route("")]
    public class DefaultController(IRequiredActor<ParentActor> actor) : ControllerBase
    {
        private readonly IActorRef _actorRef = actor.ActorRef;

        /// <summary>
        /// Redirects to the swagger-ui page.
        /// </summary>
        /// <returns>
        /// Redirect to swagger-ui.
        /// </returns>
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult Get()
        {
            return Redirect("~/swagger");
        }

        /// <summary>
        /// Writes an information log on the child.
        /// </summary>
        [HttpPost("log")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult LogInformationOnChild()
        {
            _actorRef.Tell(new ParentActor.Messages.LogMessageOnChild {  ChildName = "child-1" });

            return NoContent();
        }

        /// <summary>
        /// Throws an exception on the child.
        /// </summary>
        [HttpPost("throw")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult ThrowExceptionOnChild()
        {
            _actorRef.Tell(new ParentActor.Messages.ThrowExceptionOnChild { ChildName = "child-1" });

            return NoContent();
        }
    }
}