

using MyThreadPool.Services;
using MyThreadPool.Settings;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
 
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")  
    .CreateLogger();

builder.Host.UseSerilog();  

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<ThreadPoolSettings>(
    builder.Configuration.GetSection("ThreadPool")
);

builder.Services.AddSingleton<IThreadPool,ThreadPoolImp>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
