using Basket.API.GrpcServices;
using Basket.API.Mapper;
using Basket.API.Repositories;
using Basket.API.Repositories.Interfaces;
using Discount.Grpc.Protos;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//var connStr = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString");

//redis conf
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString");
});

builder.Services.AddScoped<IBasketRepository, BasketRepository>();

builder.Services.AddAutoMapper(typeof(BasketProfile));

//grpc conf
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>
    (o => o.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]));

builder.Services.AddScoped<DiscountGrpcService>();

//masstransit-rabbitmq configuration
builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((ctx, cfg) =>
    {
        //normalde port 5672 olmali, ancak erisim hatasindan dolayi portu 9002'ye tasidim
        //amqp rabbitmq'nun kullandigi protokol, guest'ler ise rabbitmq username ve password'u
        //cfg.Host("amqp://guest:guest@localhost:9002");
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
    });
});

//The latest version of MassTransit no longer requires the AddMassTransitHostedService configuration method.
//https://stackoverflow.com/questions/72403579/workerservice-configure-a-rabbitmq-with-masstransit
//builder.Services.AddMassTransitHostedService();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
