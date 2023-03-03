using CZ3002_Backend.Models;
using CZ3002_Backend.Repo;
using CZ3002_Backend.Services;
using Hangfire;
using Hangfire.MemoryStorage;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Development.json");

// Add services to the container.
builder.Services.AddScoped<ISampleService,SampleService>();
//builder.Services.AddSingleton<IBaseRepository<SampleUserModel>, BaseRepository<SampleUserModel>>();
builder.Services.AddScoped<ISampleUserRepository, SampleUserRepository>();
builder.Services.AddScoped<IHdbCarparkRepository, HdbCarparkRepository>();
builder.Services.AddScoped<IMallCarparkRepository, MallCarparkRepository>();
builder.Services.AddScoped<IUraCarparkRepository, UraCarparkRepository>();

builder.Services.AddScoped<IDataSetUpService<HdbCarParkModel, GovLiveCarparkDatum>,HdbCarparkDataSetUpService>();
builder.Services.AddScoped<IDataSetUpService<MallCarparkModel, LtaLiveCarparkValue>, MallCarparkDataSetUpService>();
builder.Services.AddScoped<IDataSetUpService<UraCarparkModel, UraLiveResult>, UraCarparkDataSetUpService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHangfire(c => c.UseMemoryStorage() );
builder.Services.AddHangfireServer();
JobStorage.Current = new MemoryStorage();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseHangfireDashboard();

app.UseAuthorization();

app.MapControllers();

RecurringJob.AddOrUpdate<ISampleService>("SampleJobName",x=>x.SampleFunction(),Cron.Hourly);

app.Run();

