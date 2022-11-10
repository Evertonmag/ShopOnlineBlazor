using Microsoft.AspNetCore.Components;
using ShopOnline.Models.Dtos;
using ShopOnlineMudBlazor.Web.Services.Contracts;

namespace ShopOnlineMudBlazor.Web.Pages.ProductDetails;

public class ProductDetailsBase : ComponentBase
{
    [Parameter]
    public int Id { get; set; }

    [Inject]
    public IProductService ProductService { get; set; }

    [Inject]
    public IShoppingCartService ShoppingCartService { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    public ProductDto Product { get; set; }

    public string ErrorMessage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            Product = await ProductService.GetItem(Id);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    protected async Task AddToCart(CartItemToAddDto cartItemToAddDto)
    {
        try
        {
            var cartItemDto = await ShoppingCartService.AddItem(cartItemToAddDto);

            NavigationManager.NavigateTo("/ShoppingCart");
        }
        catch (Exception)
        {
            //Log exception
        }
    }
}
