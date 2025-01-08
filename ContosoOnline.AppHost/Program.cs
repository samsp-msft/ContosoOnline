var builder = DistributedApplication.CreateBuilder(args);

var sqlserver = builder.AddSqlServer("ContosoOnlineSQL");
var catalogDb = sqlserver.AddDatabase("CatalogDbContext");
var ordersDb = sqlserver.AddDatabase("OrderDbContext");

var catalogapi = builder.AddProject<Projects.ContosoOnline_CatalogApi>("catalogapi")
                        .WithReference(catalogDb)
                        .WaitFor(catalogDb);

var orderapi = builder.AddProject<Projects.ContosoOnline_OrderApi>("orderapi")
                      .WithReference(ordersDb)
                      .WaitFor(ordersDb);

var frontend = builder.AddProject<Projects.ContosoOnline_Web>("frontend")
                      .WaitFor(orderapi)
                      .WaitFor(catalogapi)
                      .WithReference(catalogapi)
                      .WithReference(orderapi);

var orderprocessor = builder.AddProject<Projects.ContosoOnline_Workers_OrderProcessor>("orderprocessor")
                            .WaitFor(orderapi)
                            .WaitFor(catalogapi)
                            .WithReference(catalogapi)
                            .WithReference(orderapi);

builder.Build().Run();
