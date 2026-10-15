using NetArchTest.Rules;

namespace EventBank.Accounts.Architecture.Tests;

public sealed class CleanArchitectureTests
{
    private static readonly string[] Frameworks =
        ["Microsoft.EntityFrameworkCore", "Npgsql", "RabbitMQ", "MailKit", "Microsoft.AspNetCore", "EventBank.Contracts"];

    [Fact]
    public void The_domain_depends_on_nothing_but_itself_and_the_domain_building_blocks()
    {
        var result = Types.InAssembly(Layers.Domain)
            .ShouldNot()
            .HaveDependencyOnAny([
                $"{Layers.Root}.Application",
                $"{Layers.Root}.Infrastructure",
                $"{Layers.Root}.Api",
                "EventBank.BuildingBlocks.Application",
                .. Frameworks,
            ])
            .GetResult();

        result.FailingTypeNames.ShouldBeNull();
    }

    [Fact]
    public void Domain_events_are_sealed()
    {
        var result = Types.InAssembly(Layers.Domain)
            .That().ImplementInterface(typeof(EventBank.BuildingBlocks.Domain.IDomainEvent)).And().AreClasses()
            .Should().BeSealed()
            .GetResult();

        result.FailingTypeNames.ShouldBeNull();
    }

    [Fact]
    public void The_application_does_not_know_about_infrastructure_or_delivery()
    {
        var result = Types.InAssembly(Layers.Application)
            .ShouldNot()
            .HaveDependencyOnAny([
                $"{Layers.Root}.Infrastructure",
                $"{Layers.Root}.Api",
                "EventBank.BuildingBlocks.EventSourcing",
                "EventBank.BuildingBlocks.Messaging",
                .. Frameworks,
            ])
            .GetResult();

        result.FailingTypeNames.ShouldBeNull();
    }

    [Fact]
    public void The_api_talks_to_the_application_and_never_to_the_domain()
    {
        var result = Types.InAssembly(Layers.Api)
            .ShouldNot()
            .HaveDependencyOn($"{Layers.Root}.Domain")
            .GetResult();

        result.FailingTypeNames.ShouldBeNull();
    }

    [Fact]
    public void The_service_never_references_another_bounded_context()
    {
        var result = Types.InAssemblies(Layers.All)
            .ShouldNot()
            .HaveDependencyOnAny(Layers.OtherBoundedContexts)
            .GetResult();

        result.FailingTypeNames.ShouldBeNull();
    }
}
