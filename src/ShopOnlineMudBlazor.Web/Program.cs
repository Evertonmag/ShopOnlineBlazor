using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ShopOnlineMudBlazor.Web;
using MudBlazor.Services;
using ShopOnlineMudBlazor.Web.Services.Contracts;
using ShopOnlineMudBlazor.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7227") });

builder.Services.AddMudServices();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();

await builder.Build().RunAsync();
