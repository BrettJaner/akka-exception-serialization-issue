# Akka Exception Serialization Issue
Reproduces Akka.NET serialization issue with BCL exceptions in mixed .NET Core &amp; .NET Framework cluster.

See https://github.com/akkadotnet/akka.net/discussions/7193

## Running Instructions
Run both the AkkaExceptionSerializationIssue.Net8 and AkkaExceptionSerializationIssue.NetFramework481 projects/applications at the same time.  The AkkaExceptionSerializationIssue.Net8 application is also a Web API w/ a SwaggerUI page that can be used for testing.  Once the applications have started you can use the SwaggerUI page to tell the Child actor to do something.  Use the /log endpoint to test logging on the Child actor.  Use the /throw endpoint to reproduce the serialization issue.  Once the serialization issue occurs, the child is no longer responsive.

### Ports used
* 55454 (HTTP Web API for AkkaExceptionSerializationIssue.Net8)
* 55455 (Akka Remote for AkkaExceptionSerializationIssue.Net8)
* 55456 (Akka Remote for AkkaExceptionSerializationIssue.NetFramework481)
