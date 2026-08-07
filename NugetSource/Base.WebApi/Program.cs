using Base.Application;
using Base.Persistance;
using Base.WebApi;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddSwaggerServies();

//builder.Services.AddMediatR(typeof(Program));

var connectionString = builder.Configuration.GetConnectionString("Local");

builder.Services.AddApplicationServices(connectionString);
builder.Services.AddDataServices(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
