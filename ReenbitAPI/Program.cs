using Azure.Storage.Blobs;
using ReenbitAPI.Services;
using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddCors();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IBlobService, BlobService>();
builder.Services.AddSingleton(q =>
    new BlobServiceClient(builder.Configuration.GetConnectionString("StorageAccount")));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
if (app.Environment.IsDevelopment())
{
    
    app.UseSwaggerUI();
}
else
{
    app.UseSwaggerUI(s =>
    {
        s.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        s.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseCors(q => q.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().WithExposedHeaders("*"));

app.UseAuthorization();

app.MapControllers();

app.Run();
