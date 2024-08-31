

public class GlobalExceptionHandlingTests
{
    private readonly Mock<ILogger<GlobalExceptionHandling>> _mockLogger;
    private readonly GlobalExceptionHandling _middleware;
    private readonly DefaultHttpContext _httpContext;
    private readonly RequestDelegate _next;

    public GlobalExceptionHandlingTests()
    {
        _mockLogger = new Mock<ILogger<GlobalExceptionHandling>>();
        _middleware = new GlobalExceptionHandling(_mockLogger.Object);
        _httpContext = new DefaultHttpContext();
        _next = (HttpContext) => Task.CompletedTask;
    }

    [Fact(DisplayName = "Deve retornar sucesso ao invocar a middleware GlobalExceptionHandling")]
    public async Task InvokeAsync_NoException_With_Success()
    {
        await _middleware.InvokeAsync(_httpContext, _next);
        Assert.Equal((int)HttpStatusCode.OK, _httpContext.Response.StatusCode);
    }

    [Fact(DisplayName = "Deve retornar badRequest ao invocar middleware")]
    public async Task InvokeAsync_Throw_Custom_GlobalException()
    {
        var expectedException = new GlobalException(HttpStatusCode.BadRequest, "Test Global Exception");

        var next = new RequestDelegate((context) => throw expectedException);

        _httpContext.Response.Body = new MemoryStream();

        await _middleware.InvokeAsync(_httpContext, next);

        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);

        var responseBody = new StreamReader(_httpContext.Response.Body).ReadToEnd();

        Assert.Contains(expectedException.Message, responseBody);
        Assert.Equal((int)HttpStatusCode.BadRequest, _httpContext.Response.StatusCode);
    }

    [Fact(DisplayName = "Deve disparar badRequest invocar GlobalException.When")]
    public void InvokeAsync_Throw_Custom_GlobalException_When()
    {
        var result = 
            Assert.ThrowsAny<GlobalException>(() => 
                GlobalException.When(true,
                                     "Teste validar When",
                                     HttpStatusCode.BadRequest)
            );

        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Equal("Teste validar When", result.Message);
    }

    [Fact(DisplayName = "Ao disparar uma exception a middleware deve retornar com sucesso a mensagem de erro")]
    public async Task InvokeAsync_Throw_Exception_InternalServerError()
    {
        var expectedException = new Exception("Test Exception");

        var next = new RequestDelegate((context) => throw expectedException);

        _httpContext.Response.Body = new MemoryStream();

        await _middleware.InvokeAsync(_httpContext, next);

        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);

        var responseBody = new StreamReader(_httpContext.Response.Body).ReadToEnd();
        Assert.Contains("An internal server has occurred", responseBody);
        Assert.Equal((int)HttpStatusCode.InternalServerError, _httpContext.Response.StatusCode);
    }
