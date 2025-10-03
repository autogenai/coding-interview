using llmservice.Services;
using llmservice.Services.Stubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IBedrockClient, BedrockClient>();

builder.Services.AddScoped<IOpenAiClient, OpenAiClient>();

builder.Services.AddScoped<IChatService, ChatService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LLM Chat Service v1");
        c.RoutePrefix = "docs";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
