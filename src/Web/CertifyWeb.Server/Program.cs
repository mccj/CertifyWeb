using MapsterMapper;
using Scalar.AspNetCore;
using SqlSugar;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();



builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped(sp =>
{
    var connectionString = "DataSource=sqlsugar-dev.db";//builder.Configuration.GetConnectionString("");
    if (string.IsNullOrWhiteSpace(connectionString)) throw new Exception("���ݿ������ַ�������Ϊ��");
    return SqlSugarHelper.GetSqlSugarClient(SqlSugar.DbType.Sqlite, connectionString);
});
builder.Services.AddScoped(typeof(SimpleClient<>)); // �ִ�ע��

builder.Services.AddTransient<IMapper, Mapper>();

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // /scalar/v1
    app.MapScalarApiReference();
    // /swagger
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
