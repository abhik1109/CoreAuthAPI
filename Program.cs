using CoreAuthAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder = CustomServiceCollection.AddServices(builder);

var app=CustomServiceCollection.BuildHttpPipeline(builder);

app.Run();
