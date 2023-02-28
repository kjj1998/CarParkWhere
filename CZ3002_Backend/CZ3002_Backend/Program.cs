using CZ3002_Backend.Models;
using CZ3002_Backend.Repo;
using CZ3002_Backend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<ISampleService,SampleService>();
//builder.Services.AddSingleton<IBaseRepository<SampleUserModel>, BaseRepository<SampleUserModel>>();
builder.Services.AddScoped<ISampleUserRepository, SampleUserRepository>();
builder.Services.AddScoped<IHdbCarparkRepository, HdbCarparkRepository>();
builder.Services.AddScoped<IDataSetUpService<HdbCarParkModel, GovLiveCarparkDatum>,HdbCarparkDataSetUpService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();