using System.Diagnostics.Metrics;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using GameOfLife.Api.Contracts;

namespace GameOfLife.FunctionalTests;

public sealed class BoardsControllerTests(GameOfLifeApiFactory factory) : IClassFixture<GameOfLifeApiFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _client = factory.CreateClient();

    private static object BlinkerRequestBody() =>
        new
        {
            cells = new[]
            {
                new[] { 0, 0, 0 },
                new[] { 1, 1, 1 },
                new[] { 0, 0, 0 }
            }
        };

    private async Task<BoardCreatedResponse> CreateBoardAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/boards", BlinkerRequestBody());
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<BoardCreatedResponse>(JsonOptions))!;
    }

    [Fact]
    public async Task CreateBoard_returns_201_with_location_header_and_body()
    {
        var response = await _client.PostAsJsonAsync("/api/v1/boards", BlinkerRequestBody());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var body = await response.Content.ReadFromJsonAsync<BoardCreatedResponse>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal(3, body!.Width);
        Assert.Equal(3, body.Height);
        Assert.Equal(3, body.Population);
        Assert.True(Guid.TryParse(body.BoardId, out _));
    }

    [Fact]
    public async Task CreateBoard_swagger_request_example_is_a_15_by_15_glider()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");

        response.EnsureSuccessStatusCode();
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var cells = document.RootElement
            .GetProperty("paths")
            .GetProperty("/api/v1/boards")
            .GetProperty("post")
            .GetProperty("requestBody")
            .GetProperty("content")
            .GetProperty("application/json")
            .GetProperty("example")
            .GetProperty("cells");

        Assert.Equal(15, cells.GetArrayLength());
        Assert.All(cells.EnumerateArray(), row => Assert.Equal(15, row.GetArrayLength()));
        Assert.Equal(5, cells.EnumerateArray().SelectMany(row => row.EnumerateArray()).Sum(cell => cell.GetInt32()));
        Assert.Equal(1, cells[5][6].GetInt32());
        Assert.Equal(1, cells[6][7].GetInt32());
        Assert.Equal(1, cells[7][5].GetInt32());
        Assert.Equal(1, cells[7][6].GetInt32());
        Assert.Equal(1, cells[7][7].GetInt32());
    }

    [Theory]
    [InlineData("""{"cells":[[0,1],[0]]}""")] // ragged
    [InlineData("""{"cells":[[0,2]]}""")] // invalid value
    [InlineData("""{"cells":[]}""")] // empty
    public async Task CreateBoard_rejects_invalid_input_with_400_problem_details(string json)
    {
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/v1/boards", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    /// <summary>
    ///     Model binding and FluentValidation both produce 400s, and they used to produce different
    ///     bodies: different type URIs, titles, and error keys for the same class of failure.
    /// </summary>
    [Theory]
    [InlineData("""{"cells":[["a"]]}""")] // fails during model binding
    [InlineData("""{"cells":[[0,1],[0]]}""")] // fails a validation rule
    [InlineData("""{"cells":[[0,1],null]}""")]
    [InlineData("""{"cells":[null,[0,1]]}""")]
    [InlineData("""{"cells":[[0,1],null,[1,0]]}""")]
    public async Task Invalid_requests_share_one_problem_shape(string json)
    {
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/v1/boards", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(
            "https://gameoflife.example/problems/invalid-request",
            document.RootElement.GetProperty("type").GetString());
        Assert.Equal("The request is invalid.", document.RootElement.GetProperty("title").GetString());
        Assert.True(document.RootElement.TryGetProperty("errors", out _));
    }

    /// <summary>
    ///     The body parameter and <c>Cells</c> are both non-nullable, so MVC rejects these during binding
    ///     and neither the validator nor the controller carries a null case.
    /// </summary>
    [Theory]
    [InlineData("")] // no body at all
    [InlineData("null")] // a literal JSON null
    [InlineData("""{"cells":null}""")]
    [InlineData("{}")] // cells absent
    public async Task CreateBoard_rejects_a_null_or_absent_body(string json)
    {
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/v1/boards", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(
            "https://gameoflife.example/problems/invalid-request",
            document.RootElement.GetProperty("type").GetString());

        // An empty key is not something a client can act on, so the factory rebinds it to "body".
        foreach (var error in document.RootElement.GetProperty("errors").EnumerateObject()) Assert.NotEmpty(error.Name);
    }

    [Fact]
    public async Task A_binding_failure_does_not_leak_framework_internals()
    {
        using var content = new StringContent("""{"cells":[["a"]]}""", Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/v1/boards", content);

        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.DoesNotContain("System.", body, StringComparison.Ordinal);
        Assert.DoesNotContain("BytePositionInLine", body, StringComparison.Ordinal);
        Assert.DoesNotContain("LineNumber", body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetBoard_returns_the_seed_that_was_uploaded()
    {
        var created = await CreateBoardAsync();

        var response = await _client.GetAsync($"/api/v1/boards/{created.BoardId}");
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<BoardResponse>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal(created.BoardId, body!.BoardId);

        // Compare the cells, not just the row count. The blinker is asymmetric under transposition,
        // so this also pins the orientation end to end: a row/column swap anywhere between parsing
        // and rendering would return the vertical blinker instead.
        Assert.Equal(new[] { new[] { 0, 0, 0 }, new[] { 1, 1, 1 }, new[] { 0, 0, 0 } }, body.Cells);
    }

    [Fact]
    public async Task GetBoard_returns_404_problem_details_for_an_unknown_id()
    {
        var response = await _client.GetAsync($"/api/v1/boards/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetGeneration_returns_etag_and_immutable_cache_control()
    {
        var created = await CreateBoardAsync();

        var response = await _client.GetAsync($"/api/v1/boards/{created.BoardId}/generations/1");
        response.EnsureSuccessStatusCode();

        Assert.NotNull(response.Headers.ETag);
        Assert.Contains("immutable", response.Headers.CacheControl?.ToString());

        var body = await response.Content.ReadFromJsonAsync<GenerationResponse>(JsonOptions);
        Assert.Equal(1, body!.Generation);
    }

    [Fact]
    public async Task Generation_response_carries_links_under_the_underscore_links_key()
    {
        var created = await CreateBoardAsync();

        using var document = JsonDocument.Parse(
            await _client.GetStringAsync($"/api/v1/boards/{created.BoardId}/generations/1"));
        var links = document.RootElement.GetProperty("_links");

        Assert.Equal($"/api/v1/boards/{created.BoardId}/generations/1", links.GetProperty("self").GetString());
        Assert.Equal($"/api/v1/boards/{created.BoardId}/generations/2", links.GetProperty("next").GetString());
        Assert.Equal($"/api/v1/boards/{created.BoardId}/final", links.GetProperty("final").GetString());
        Assert.Equal($"/api/v1/boards/{created.BoardId}", links.GetProperty("board").GetString());
    }

    [Fact]
    public async Task A_route_that_fails_the_guid_constraint_still_returns_problem_details()
    {
        var response = await _client.GetAsync("/api/v1/boards/not-a-guid");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetGeneration_honours_if_none_match_with_304()
    {
        var created = await CreateBoardAsync();

        var first = await _client.GetAsync($"/api/v1/boards/{created.BoardId}/generations/1");
        var etag = first.Headers.ETag!.ToString();

        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/boards/{created.BoardId}/generations/1");
        request.Headers.TryAddWithoutValidation("If-None-Match", etag);
        var second = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotModified, second.StatusCode);
    }

    /// <summary>
    ///     The point of deriving the validator from identity rather than content: a conditional request
    ///     must be answerable without evolving anything. Asserted on the generations-computed counter
    ///     rather than on elapsed time, so it states the actual claim instead of a timing coincidence.
    /// </summary>
    [Theory]
    [InlineData("strong", "generations/25", 25)]
    [InlineData("weak", "generations/25", 25)]
    [InlineData("wildcard", "generations/25", 25)]
    [InlineData("list", "generations/25", 25)]
    [InlineData("weak", "next", 1)]
    [InlineData("wildcard", "next", 1)]
    public async Task Conditional_generation_request_does_not_run_the_evolution_loop(string validator, string route,
        int generation)
    {
        var created = await CreateBoardAsync();
        var url = $"/api/v1/boards/{created.BoardId}/{route}";

        var generationsComputed = 0L;
        using var listener = new MeterListener
        {
            InstrumentPublished = (instrument, l) =>
            {
                if (instrument.Name == "gameoflife.generations_computed") l.EnableMeasurementEvents(instrument);
            }
        };
        listener.SetMeasurementEventCallback<long>((_, measurement, _, _) =>
            Interlocked.Add(ref generationsComputed, measurement));
        listener.Start();

        var unconditional = await _client.GetAsync(url);
        var afterUnconditional = Interlocked.Read(ref generationsComputed);
        var etag = unconditional.Headers.ETag!.ToString();

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.TryAddWithoutValidation("If-None-Match", validator switch
        {
            "weak" => $"W/{etag}",
            "wildcard" => "*",
            "list" => $"\"different\", W/{etag}",
            _ => etag
        });
        var conditional = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotModified, conditional.StatusCode);
        Assert.Equal(generation, afterUnconditional);
        Assert.Equal(afterUnconditional, Interlocked.Read(ref generationsComputed));
        Assert.Equal(unconditional.Headers.ETag, conditional.Headers.ETag);
        Assert.Contains("immutable", conditional.Headers.CacheControl?.ToString());
        Assert.Empty(await conditional.Content.ReadAsByteArrayAsync());
    }

    /// <summary>
    ///     A content-derived validator collided here: a blinker at generations 0 and 2 has identical
    ///     cells, but they are different representations with different `generation` and `_links` values.
    /// </summary>
    [Fact]
    public async Task Generations_with_identical_cells_still_get_distinct_etags()
    {
        var created = await CreateBoardAsync();

        var zero = await _client.GetAsync($"/api/v1/boards/{created.BoardId}/generations/0");
        var two = await _client.GetAsync($"/api/v1/boards/{created.BoardId}/generations/2");

        var cellsAtZero = (await zero.Content.ReadFromJsonAsync<GenerationResponse>(JsonOptions))!.Cells;
        var cellsAtTwo = (await two.Content.ReadFromJsonAsync<GenerationResponse>(JsonOptions))!.Cells;

        Assert.Equal(cellsAtZero, cellsAtTwo);
        Assert.NotEqual(zero.Headers.ETag!.Tag, two.Headers.ETag!.Tag);
    }

    [Theory]
    [InlineData("strong")]
    [InlineData("weak")]
    [InlineData("wildcard")]
    public async Task A_fabricated_validator_for_an_unknown_board_is_404_not_304(string validator)
    {
        var unknown = Guid.NewGuid();

        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/boards/{unknown}/generations/1");
        request.Headers.TryAddWithoutValidation("If-None-Match", validator switch
        {
            "weak" => $"W/\"v1-{unknown}-1\"",
            "wildcard" => "*",
            _ => $"\"v1-{unknown}-1\""
        });
        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1001)]
    public async Task GetGeneration_rejects_an_out_of_range_index_with_400(int generation)
    {
        var created = await CreateBoardAsync();

        using var request = new HttpRequestMessage(HttpMethod.Get,
            $"/api/v1/boards/{created.BoardId}/generations/{generation}");
        request.Headers.TryAddWithoutValidation("If-None-Match", "*");
        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetNext_is_an_alias_for_generation_one()
    {
        var created = await CreateBoardAsync();

        var next = await _client.GetFromJsonAsync<GenerationResponse>($"/api/v1/boards/{created.BoardId}/next",
            JsonOptions);
        var generationOne =
            await _client.GetFromJsonAsync<GenerationResponse>($"/api/v1/boards/{created.BoardId}/generations/1",
                JsonOptions);

        Assert.Equal(generationOne!.Cells, next!.Cells);
    }

    [Fact]
    public async Task GetFinalState_returns_converged_result_for_an_oscillator()
    {
        var created = await CreateBoardAsync();

        var response = await _client.GetAsync($"/api/v1/boards/{created.BoardId}/final");
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<FinalStateResponse>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal(0, body!.StabilizedAtGeneration);
        Assert.Equal(2, body.Period);
        Assert.Equal(3, body.Population);
    }

    [Fact]
    public async Task A_nonmatching_weak_validator_returns_the_generation()
    {
        var created = await CreateBoardAsync();
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/boards/{created.BoardId}/generations/1");
        request.Headers.TryAddWithoutValidation("If-None-Match", "W/\"different\"");

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<GenerationResponse>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal(1, body.Generation);
    }

    [Fact]
    public async Task GetFinalState_counts_search_and_verification_steps()
    {
        var created = await _client.PostAsJsonAsync("/api/v1/boards", new { cells = new[] { new[] { 1 } } });
        created.EnsureSuccessStatusCode();
        Assert.NotNull(created.Headers.Location);

        long generationsComputed = 0;
        using var listener = new MeterListener
        {
            InstrumentPublished = (instrument, l) =>
            {
                if (instrument.Name == "gameoflife.generations_computed") l.EnableMeasurementEvents(instrument);
            }
        };
        listener.SetMeasurementEventCallback<long>((_, measurement, _, _) =>
            Interlocked.Add(ref generationsComputed, measurement));
        listener.Start();

        var response = await _client.GetAsync($"{created.Headers.Location}/final");

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<FinalStateResponse>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal(1, body.StabilizedAtGeneration);
        Assert.Equal(1, body.Period);
        Assert.Equal(0, body.Population);
        Assert.Equal(new[] { new[] { 0 } }, body.Cells);
        Assert.Equal(3, Interlocked.Read(ref generationsComputed));
    }

    [Fact]
    public async Task GetFinalState_returns_404_for_an_unknown_board()
    {
        var response = await _client.GetAsync($"/api/v1/boards/{Guid.NewGuid()}/final");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Health_endpoints_are_split_between_live_and_ready()
    {
        var live = await _client.GetAsync("/health/live");
        var ready = await _client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.OK, live.StatusCode);
        Assert.Equal(HttpStatusCode.OK, ready.StatusCode);
    }
}
