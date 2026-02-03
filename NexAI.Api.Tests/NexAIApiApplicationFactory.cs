using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NexAI.Agents;
using NexAI.Api.Tests.Fakes;
using NexAI.LLMs.Common;

namespace NexAI.Api.Tests;

public sealed class NexAIApiApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<Chat>(new FakeChat());
            services.AddSingleton<INexAIAgent>(new FakeNexAIAgent());
        });
    }
}
