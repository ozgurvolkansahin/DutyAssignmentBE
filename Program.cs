using DutyAssignment.Config.BsonMapper;
using DutyAssignment.Mapper;
using DutyAssignment.Models;
using DutyAssignment.Repositories.Mongo.Duty;
using DutyAssignment.Services;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
BsonMapper.Map();
// ClassMapRegisterer.RegisterClassMaps();
// register excelpackage license
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
builder.Services.Configure<DutyAssignmentDatabaseSettings>(
    builder.Configuration.GetSection("DutyAssignmentDatabase"));
    
builder.Services.AddScoped<IDutyService,DutyService>();
builder.Services.AddScoped<IDutyRepository,DutyRepository>();
builder.Services.AddScoped<IPersonalRepository,PersonalRepository>();
builder.Services.AddScoped<IAssignmentRepository,AssignmentRepository>();
builder.Services.AddScoped<IExcelService,ExcelService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
