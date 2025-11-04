using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();

var sampleTodos = new Todo[]
{
    new(1, "Walk the dog"),
    new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
    new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
    new(4, "Clean the bathroom"),
    new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2))),
    new(6, "Kiss the wife", DateOnly.FromDateTime(DateTime.Now.AddDays(3)))
};

var todosApi = app.MapGroup("/todos");
todosApi.MapGet("/", () => sampleTodos);
todosApi.MapGet("/{id}", (int id) =>
    sampleTodos.FirstOrDefault(a => a.Id == id) is { } todo
        ? Results.Ok(todo)
        : Results.NotFound());

app.MapGet("/", () =>
{
    // Using Results.Content allows us to specify the content type as 'text/html'.
    // This creates a more helpful landing page for developers exploring your API.
    var html = """
                   <!DOCTYPE html>
                   <html lang="en">
                   <head>
                       <meta charset="UTF-8">
                       <title>Todo API</title>
                       <style>
                           body { font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Helvetica, Arial, sans-serif; line-height: 1.6; padding: 2em; color: #333; }
                           h1 { color: #000; }
                           a { text-decoration: none; color: #007bff; }
                           a:hover { text-decoration: underline; }
                           code { background-color: #f2f2f2; padding: 2px 6px; border-radius: 4px; font-family: "SFMono-Regular", Consolas, "Liberation Mono", Menlo, monospace; }
                       </style>
                   </head>
                   <body>
                       <h1>Welcome to the Todo API</h1>
                       <p>This is a sample minimal API for managing a list of todos.</p>
                       <p>
                           You can interact with the API at the <code>/todos</code> endpoint.
                           <br>
                           Try it now: <a href="/todos">/todos</a>
                       </p>
                   </body>
                   </html>
               """;
    return Results.Content(html, "text/html");
});


app.Run();

public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

[JsonSerializable(typeof(Todo[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}