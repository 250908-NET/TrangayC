namespace TaskManagementApi.Tests;

public class EndpointsTests
{
    private static WebApplicationFactory<Program> CreateFactory()
        => new WebApplicationFactory<Program>();

    [Fact]
    public async Task GetTasks_Empty_ReturnsOk_WithEmptyList()
    {
        // Arrange
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/tasks");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseJson = await response.Content.ReadFromJsonAsync<JsonElement>();
        responseJson.GetProperty("success").GetBoolean().Should().BeTrue();
        responseJson.GetProperty("data").EnumerateArray().Should().BeEmpty();
    }

    [Fact]
    public async Task GetTasks_ReturnsOk_WithFiltering()
    {
        // Arrange
        using var factory = CreateFactory();
        using var client = factory.CreateClient();
        await client.PostAsJsonAsync("/api/tasks", new { title = "A", isCompleted = false, priority = 0 });
        await client.PostAsJsonAsync("/api/tasks", new { title = "B", isCompleted = true, priority = 1 });
        await client.PostAsJsonAsync("/api/tasks", new { title = "C", isCompleted = true, priority = 1, dueDate = DateTime.Now });

        // Act - filter by isCompleted=true & priority=1
        var response = await client.GetAsync("/api/tasks?isCompleted=true&priority=1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseJson = await response.Content.ReadFromJsonAsync<JsonElement>();
        var tasksArray = responseJson.GetProperty("data").EnumerateArray().ToArray();
        tasksArray.Length.Should().BeGreaterOrEqualTo(2);
        tasksArray.All(t => t.GetProperty("isCompleted").GetBoolean() && t.GetProperty("priority").GetInt32() == 1).Should().BeTrue();
    }

    [Fact]
    public async Task GetTasks_ReturnsOk_WithBody()
    {
        // Arrange
        using var factory = CreateFactory();
        using var client = factory.CreateClient();
        await client.PostAsJsonAsync("/api/tasks", new { title = "A", isCompleted = false, priority = 1, dueDate = DateTime.Now });

        // Act
        var response = await client.GetAsync("/api/tasks");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseJson = await response.Content.ReadFromJsonAsync<JsonElement>();
        responseJson.GetProperty("success").GetBoolean().Should().BeTrue();
        responseJson.GetProperty("data").EnumerateArray().Should().NotBeEmpty();
        responseJson.GetProperty("message").GetString().Should().Be("Operation completed successfully");
    }

    [Fact]
    public async Task GetTask_ById_ReturnsNotFound_WithBody()
    {
        // Arrange
        using var factory = CreateFactory();
        using var client = factory.CreateClient();
        const int taskId = 9999;
        await client.PostAsJsonAsync("/api/tasks", new { title = "A", isCompleted = false, priority = 0 });

        // Act
        var response = await client.GetAsync($"/api/tasks/{taskId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var responseJson = await response.Content.ReadFromJsonAsync<JsonElement>();
        responseJson.GetProperty("success").GetBoolean().Should().BeFalse();
        responseJson.GetProperty("error").GetString().Should().Contain(taskId.ToString());
        responseJson.GetProperty("message").GetString().Should().Be("Operation failed");
    }

    [Fact]
    public async Task GetTask_ById_ReturnsOk_WithBody()
    {
        // Arrange
        using var factory = CreateFactory();
        using var client = factory.CreateClient();
        await client.PostAsJsonAsync("/api/tasks", new { title = "A", isCompleted = false, priority = 1, dueDate = DateTime.Now });
        await client.PostAsJsonAsync("/api/tasks", new { title = "B", isCompleted = true, priority = 2, dueDate = DateTime.Now });
        const int taskId = 1;

        // Act
        var response = await client.GetAsync($"/api/tasks/{taskId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseJson = await response.Content.ReadFromJsonAsync<JsonElement>();
        responseJson.GetProperty("success").GetBoolean().Should().BeTrue();
        responseJson.GetProperty("data").GetProperty("title").GetString().Should().Be("A");
        responseJson.GetProperty("message").GetString().Should().Be("Operation completed successfully");
    }

    [Fact]
    public async Task PostTask_TitleEmpty_ReturnsBadRequest_WithErrors()
    {
        // Arrange
        using var factory = CreateFactory();
        using var client = factory.CreateClient();
        var body = new { title = "" }; 

        // Act
        var response = await client.PostAsJsonAsync("/api/tasks", body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseJson = await response.Content.ReadFromJsonAsync<JsonElement>();
        responseJson.GetProperty("success").GetBoolean().Should().BeFalse();
        var errorMessages = responseJson.GetProperty("errors").EnumerateArray().ToArray();
        errorMessages.Length.Should().BeGreaterOrEqualTo(1);
        errorMessages.All(e => e.GetString() == "Title is required").Should().BeTrue();
    }

    [Fact]
    public async Task PostTask_TitleTooLong_ReturnsBadRequest_WithErrors()
    {
        // Arrange
        using var factory = CreateFactory();
        using var client = factory.CreateClient();
        var longTitle = new string('a', 101); // > 100
        var body = new { title = longTitle };

        // Act
        var response = await client.PostAsJsonAsync("/api/tasks", body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseJson = await response.Content.ReadFromJsonAsync<JsonElement>();
        responseJson.GetProperty("success").GetBoolean().Should().BeFalse();
        var errorMessages = responseJson.GetProperty("errors").EnumerateArray().ToArray();
        errorMessages.Length.Should().BeGreaterOrEqualTo(1);
        errorMessages.All(e => e.GetString() == "Title must not exceed 100 characters").Should().BeTrue();
    }

    [Fact]
    public async Task PostTask_DescriptionTooLong_ReturnsBadRequest_WithErrors()
    {
        // Arrange
        using var factory = CreateFactory();
        using var client = factory.CreateClient();
        var longDesc = new string('b', 501); // > 500
        var body = new { title = "A", description = longDesc };

        // Act
        var response = await client.PostAsJsonAsync("/api/tasks", body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseJson = await response.Content.ReadFromJsonAsync<JsonElement>();
        responseJson.GetProperty("success").GetBoolean().Should().BeFalse();
        var errorMessages = responseJson.GetProperty("errors").EnumerateArray().ToArray();
        errorMessages.Length.Should().BeGreaterOrEqualTo(1);
        errorMessages.All(e => e.GetString() == "Description must not exceed 500 characters").Should().BeTrue();
    }

    [Fact]
    public async Task PostTask_ReturnsCreated_WithBody()
    {
        // Arrange
        using var factory = CreateFactory();
        using var client = factory.CreateClient();
        var body = new
        {
            title = "A",
            description = "Just testing",
            isCompleted = false,
            priority = 1,
            dueDate = DateTime.Now.AddDays(7)
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/tasks", body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var responseJson = await response.Content.ReadFromJsonAsync<JsonElement>();
        responseJson.GetProperty("success").GetBoolean().Should().BeTrue();
        responseJson.GetProperty("data").GetProperty("title").GetString().Should().Be("A");
        responseJson.GetProperty("data").GetProperty("description").GetString().Should().Be("Just testing");
        responseJson.GetProperty("message").GetString().Should().Be("Operation completed successfully");
    }

    [Fact]
    public async Task PutTask_ReturnsNotFound_WithBody()
    {
        // Arrange
        using var factory = CreateFactory();
        using var client = factory.CreateClient();
        const int taskId = 123;
        var body = new { title = "Update" };

        // Act
        var response = await client.PutAsJsonAsync($"/api/tasks/{taskId}", body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var responseJson = await response.Content.ReadFromJsonAsync<JsonElement>();
        responseJson.GetProperty("success").GetBoolean().Should().BeFalse();
        responseJson.GetProperty("error").GetString().Should().Contain(taskId.ToString());
        responseJson.GetProperty("message").GetString().Should().Be("Operation failed");
    }

    [Fact]
    public async Task PutTask_ReturnsOk_WithBody()
    {
        // Arrange
        using var factory = CreateFactory(); 
        using var client = factory.CreateClient();
        const int taskId = 1; 
        var body = new { title = "Update" };
        await client.PostAsJsonAsync("/api/tasks", body);

        // Act 
        var response = await client.PutAsJsonAsync($"/api/tasks/{taskId}", body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseJson = await response.Content.ReadFromJsonAsync<JsonElement>();
        responseJson.GetProperty("success").GetBoolean().Should().BeTrue();
        responseJson.GetProperty("data").GetProperty("title").GetString().Should().Be("Update");
        responseJson.GetProperty("message").GetString().Should().Be("Operation completed successfully");
    }

    [Fact]
    public async Task DeleteTask_ReturnsNotFound_WithBody()
    {
        // Arrange
        using var factory = CreateFactory();
        using var client = factory.CreateClient();
        const int taskId = 1000;

        // Act
        var response = await client.DeleteAsync($"/api/tasks/{taskId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var responseJson = await response.Content.ReadFromJsonAsync<JsonElement>();
        responseJson.GetProperty("success").GetBoolean().Should().BeFalse();
        responseJson.GetProperty("error").GetString().Should().Contain(taskId.ToString());
        responseJson.GetProperty("message").GetString().Should().Be("Operation failed");
    }

    [Fact]
    public async Task DeleteTask_ReturnsOk_WithBody()
    {
        // Arrange
        using var factory = CreateFactory();
        using var client = factory.CreateClient();
        await client.PostAsJsonAsync("/api/tasks", new { title = "Delete" });
        const int taskId = 1;

        // Act
        var response = await client.DeleteAsync($"/api/tasks/{taskId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseJson = await response.Content.ReadFromJsonAsync<JsonElement>();
        responseJson.GetProperty("success").GetBoolean().Should().BeTrue();
        responseJson.GetProperty("data").GetProperty("title").GetString().Should().Be("Delete");
        responseJson.GetProperty("message").GetString().Should().Be("Operation completed successfully");
    }

    [Fact]
    public async Task GetStatistics_ReturnsOk_WithCorrectCountsAndGrouping()
    {
        // Arrange
        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        // Create tasks: 3 total; 1 completed; 1 overdue
        await client.PostAsJsonAsync("/api/tasks", new { title = "A", isCompleted = false, priority = 0, dueDate = DateTime.Now.AddDays(3) }); // Low
        await client.PostAsJsonAsync("/api/tasks", new { title = "B", isCompleted = true, priority = 2, dueDate = DateTime.Now.AddDays(1) }); // High
        await client.PostAsJsonAsync("/api/tasks", new { title = "C", isCompleted = false, priority = 2, dueDate = DateTime.Now.AddDays(-1) }); // High overdue

        // Act
        var response = await client.GetAsync("/api/tasks/statistics");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseJson = await response.Content.ReadFromJsonAsync<JsonElement>();
        responseJson.GetProperty("success").GetBoolean().Should().BeTrue();
        var data = responseJson.GetProperty("data");
        data.GetProperty("totalTasks").GetInt32().Should().Be(3);
        data.GetProperty("completedTasks").GetInt32().Should().Be(1);
        data.GetProperty("overdueTasks").GetInt32().Should().Be(1);

        var tasksByPriority = data.GetProperty("tasksByPriority").EnumerateArray().ToArray();
        var low = tasksByPriority.Single(x => x.GetProperty("priority").GetString() == "Low");
        low.GetProperty("count").GetInt32().Should().Be(1);
        var high = tasksByPriority.Single(x => x.GetProperty("priority").GetString() == "High");
        high.GetProperty("count").GetInt32().Should().Be(2);
    }
}
