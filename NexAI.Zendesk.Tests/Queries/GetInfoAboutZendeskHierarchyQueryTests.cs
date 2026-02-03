using NexAI.Tests.Neo4j;
using NexAI.Zendesk.Commands;
using NexAI.Zendesk.Queries;

namespace NexAI.Zendesk.Tests.Queries;

[Collection("Neo4j")]
public class GetInfoAboutZendeskHierarchyQueryTests(Neo4jTestFixture fixture) : Neo4jDbBasedTest(fixture)
{
  [Fact]
    public async Task Handle_WithHierarchyData_ReturnsQueryResult()
    {
        // arrange
        var upsertGroupCommand = new UpsertZendeskGroupCommand(Neo4jDbClient);
        var upsertUserCommand = new UpsertZendeskUserCommand(Neo4jDbClient);
        var upsertMembersCommand = new UpsertZendeskMembersOfRelationshipCommand(Neo4jDbClient);

        var group1 = new ZendeskGroup(ZendeskGroupId.New(), "group-1", "Test Group One");
        await upsertGroupCommand.Handle(group1);
        var user11 = new ZendeskUser(ZendeskUserId.New(), "user-11", "User 11", "user11@example.com");
        await upsertUserCommand.Handle(user11);
        await upsertMembersCommand.Handle(new ZendeskUserGroups(user11.Id, [group1.Id]));
        var user12 = new ZendeskUser(ZendeskUserId.New(), "user-12", "User 12", "user12@example.com");
        await upsertUserCommand.Handle(user12);
        await upsertMembersCommand.Handle(new ZendeskUserGroups(user12.Id, [group1.Id]));

        var group2 = new ZendeskGroup(ZendeskGroupId.New(), "group-2", "Test Group Two");
        await upsertGroupCommand.Handle(group2);
        var user21 = new ZendeskUser(ZendeskUserId.New(), "user-21", "User 21", "user21@example.com");
        await upsertUserCommand.Handle(user21);
        await upsertMembersCommand.Handle(new ZendeskUserGroups(user21.Id, [group2.Id]));
        var user22 = new ZendeskUser(ZendeskUserId.New(), "user-22", "User 22", "user22@example.com");
        await upsertUserCommand.Handle(user22);
        await upsertMembersCommand.Handle(new ZendeskUserGroups(user22.Id, [group2.Id]));

        var query = new GetInfoAboutZendeskHierarchyQuery(Neo4jDbClient);
        const string cypherQuery = "MATCH (group:Group)<-[member_of:MEMBER_OF]-(user:User) RETURN group, user, member_of";

        // act
        var result = await query.Handle(cypherQuery);

        // assert
        var expected = $@"[
  {{
    ""group"": {{
      ""labels"": [""Group""],
      ""properties"": {{
        ""name"": ""{group1.Name}"",
        ""id"": ""{group1.Id}"",
        ""zendeskId"": ""{group1.ExternalId}""
      }}
    }},
    ""user"": {{
      ""labels"": [""User""],
      ""properties"": {{
        ""name"": ""{user11.Name}"",
        ""id"": ""{user11.Id}"",
        ""zendeskId"": ""{user11.ExternalId}"",
        ""email"": ""{user11.Email}""
      }}
    }},
    ""member_of"": {{
      ""type"": ""MEMBER_OF"",
      ""properties"": {{}}
    }}
  }},
  {{
    ""group"": {{
      ""labels"": [""Group""],
      ""properties"": {{
        ""name"": ""{group1.Name}"",
        ""id"": ""{group1.Id}"",
        ""zendeskId"": ""{group1.ExternalId}""
      }}
    }},
    ""user"": {{
      ""labels"": [""User""],
      ""properties"": {{
        ""name"": ""{user12.Name}"",
        ""id"": ""{user12.Id}"",
        ""zendeskId"": ""{user12.ExternalId}"",
        ""email"": ""{user12.Email}""
      }}
    }},
    ""member_of"": {{
      ""type"": ""MEMBER_OF"",
      ""properties"": {{}}
    }}
  }}, 
  {{
    ""group"": {{
      ""labels"": [""Group""],
      ""properties"": {{
        ""name"": ""{group2.Name}"",
        ""id"": ""{group2.Id}"",
        ""zendeskId"": ""{group2.ExternalId}""
      }}
    }},
    ""user"": {{
      ""labels"": [""User""],
      ""properties"": {{
        ""name"": ""{user21.Name}"",
        ""id"": ""{user21.Id}"",
        ""zendeskId"": ""{user21.ExternalId}"",
        ""email"": ""{user21.Email}""
      }}
    }},
    ""member_of"": {{
      ""type"": ""MEMBER_OF"",
      ""properties"": {{}}
    }}
  }}, 
  {{
    ""group"": {{
      ""labels"": [""Group""],
      ""properties"": {{
        ""name"": ""{group2.Name}"",
        ""id"": ""{group2.Id}"",
        ""zendeskId"": ""{group2.ExternalId}""
      }}
    }},
    ""user"": {{
      ""labels"": [""User""],
      ""properties"": {{
        ""name"": ""{user22.Name}"",
        ""id"": ""{user22.Id}"",
        ""zendeskId"": ""{user22.ExternalId}"",
        ""email"": ""{user22.Email}""
      }}
    }},
    ""member_of"": {{
      ""type"": ""MEMBER_OF"",
      ""properties"": {{}}
    }}
  }}
]
";
        result.Replace("\r", "").Replace("\n", "").Replace(" ", "")
          .Should().Be(expected.Replace("\r", "").Replace("\n", "").Replace(" ", ""));
    }
    
    [Fact]
    public async Task Handle_WithoutData_ReturnsQueryResult()
    {
        // arrange
        var query = new GetInfoAboutZendeskHierarchyQuery(Neo4jDbClient);
        const string cypherQuery = "MATCH (group:Group)<-[:MEMBER_OF]-(user:User) RETURN group, user";

        // act
        var result = await query.Handle(cypherQuery);

        // assert
        result.Should().Be("[]");
    }

    [Fact]
    public async Task Handle_WithInvalidCypherQuery_ReturnsErrorMessage()
    {
        // arrange
        var query = new GetInfoAboutZendeskHierarchyQuery(Neo4jDbClient);
        const string invalidCypherQuery = "MATCH (invalid syntax here";

        // act
        var result = await query.Handle(invalidCypherQuery);

        // assert
        result.Should().NotBeNullOrEmpty();
    }
}