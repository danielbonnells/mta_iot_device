var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Load configuration from appsettings.json
//builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Logging.AddConsole();

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