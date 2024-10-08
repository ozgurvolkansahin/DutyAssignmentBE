using DutyAssignment.Config.BsonMapper;
using DutyAssignment.Mapper;
using DutyAssignment.Models;
using DutyAssignment.Repositories.Mongo.Duty;
using DutyAssignment.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
BsonMapper.Map();
// ClassMapRegisterer.RegisterClassMaps();
builder.Services.Configure<DutyAssignmentDatabaseSettings>(
    builder.Configuration.GetSection("DutyAssignmentDatabase"));
    
builder.Services.AddScoped<IDutyService,DutyService>();
builder.Services.AddScoped<IDutyRepository,DutyRepository>();

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
