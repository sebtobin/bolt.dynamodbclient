﻿using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Bolt.Common.Extensions;
using Bolt.DynamoDbClient.Tests.Builders;
using Bolt.DynamoDbClient.Tests.TestHelpers;
using Shouldly;

namespace Bolt.DynamoDbClient.Tests;

public class DynamoDbWrapper_Query_Should
{
    private IDynamoDbWrapper sut;
    private IAmazonDynamoDB fake;

    public DynamoDbWrapper_Query_Should()
    {
        fake = Substitute.For<IAmazonDynamoDB>();
        sut = DynamoDbWrapperBuilder.Build(fake);
    }

    [Fact]
    public async void Generate_correct_input_for_equal_search()
    {
        QueryRequest? gotRequest = null;
        fake.QueryAsync(Arg.Any<QueryRequest>(), Arg.Any<CancellationToken>())
            .Returns(new QueryResponse 
            { 
                Count = 1,
                HttpStatusCode = System.Net.HttpStatusCode.OK,
                ScannedCount = 1,
                Items = new List<Dictionary<string, AttributeValue>>
                {
                    new Dictionary<string, AttributeValue>
                    {
                        ["PK"] = new AttributeValue
                        {
                            S = "book-1"
                        },
                        ["SK"] = new AttributeValue
                        {
                            S = "details"
                        },
                        ["Title"] = new AttributeValue
                        {
                            S = "book-title"
                        },
                        ["Price"] = new AttributeValue
                        {
                            N = "100.34"
                        },
                        ["Identifier"] = new AttributeValue
                        {
                            S = "8359FC3F-B5EE-4622-9993-2B5CFAE03A1E"
                        },
                        ["PublishedAt"] = new AttributeValue
                        {
                            S = "2022-12-31T13:00:00Z".ToUtcDateTime().FormatAsUtc()
                        }
                    }
                }
            })
            .AndDoes(c => gotRequest = c.Arg<QueryRequest>());

        var gotRsp = await sut.Query()
            .PartitionKey().Equals("partition-key")
            .SortKey().Equals("sort-key")
            .Fetch<BookDbRecord>();

        new
        {
            gotRsp,
            gotRequest
        }.ShouldMatchApproved();
    }
}

