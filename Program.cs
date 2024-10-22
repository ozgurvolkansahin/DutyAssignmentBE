using DutyAssignment.Config.BsonMapper;
using DutyAssignment.Mapper;
using DutyAssignment.Models;
using DutyAssignment.Repositories.Mongo.Duty;
using DutyAssignment.Services;
using DutyAssignment.Configuration.Config;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
BsonMapper.Map();
DotNetEnv.Env.Load();
Console.WriteLine($"Loading configuration from {Config.MongoUrl}");
Console.WriteLine($"MONGO_URL: {DotNetEnv.Env.GetString("MONGO_URL")}");
Console.WriteLine($"Current Directory: {Directory.GetCurrentDirectory()}");

// setup cors
var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder.WithOrigins("http://localhost:3000",
                          "localhost:8080");
                          builder.AllowAnyHeader();
                            builder.AllowAnyMethod();
                      });
});
// ClassMapRegisterer.RegisterClassMaps();
// register excelpackage license
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
builder.Services.Configure<DutyAssignmentDatabaseSettings>(
    builder.Configuration.GetSection("DutyAssignmentDatabase"));
    
builder.Services.AddScoped<IDutyService,DutyService>();
builder.Services.AddScoped<IDutyRepository,DutyRepository>();
builder.Services.AddScoped<IPersonalRepository,PersonalRepository>();
builder.Services.AddScoped<IAssignmentRepository,AssignmentRepository>();
builder.Services.AddScoped<IDashboardRepository,DashboardRepository>();

builder.Services.AddScoped<IExcelService,ExcelService>();
builder.Services.AddScoped<IAssignmentService,AssignmentService>();
builder.Services.AddScoped<IDashboardService,DashboardService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
