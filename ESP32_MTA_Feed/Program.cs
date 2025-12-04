using ESP32_MTA_Feed;
using ESP32_MTA_Feed.Services;
using MySqlConnector;
using Pomelo.EntityFrameworkCore.MySql;

var builder = WebApplication.CreateBuilder(args);

// builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddSingleton<MqttService>(provider =>
    {
        var configuration = provider.GetRequiredService<IConfiguration>();
        return  MqttService.Instance(configuration);
    });
builder.Services.AddSingleton<StopService>(provider =>
    {
        var configuration = provider.GetRequiredService<IConfiguration>();
        return  StopService.Instance(configuration);
    });
builder.Services.AddHostedService<MqttPublishScheduler>();
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

// app.UseHttpsRedirection();

//app.UseAuthorization();



app.UseForwardedHeaders();

app.MapControllers();



app.Run();

//info
//https://www.digitalocean.com/community/tutorials/how-to-deploy-an-asp-net-core-application-with-mysql-server-using-nginx-on-ubuntu-18-04