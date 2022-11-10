using Microsoft.AspNetCore.Components;
using ShopOnline.Models.Dtos;
using ShopOnline.Web.Services.Contracts;

namespace ShopOnline.Web.Pages.Products;
public class ProductsBase : ComponentBase
{
    [Inject]
    public IProductService ProductService { get; set; }

    [Inject]
    public IShoppingCartService ShoppingCartService { get; set; }

    [Inject]
    public IManageProductsLocalStorageService ManageProductsLocalStorageService { get; set; }

    [Inject]
    public IManageCartItemsLocalStorageService ManageCartItemsLocalStorageService { get; set; }

    public IEnumerable<ProductDto> Products { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    public string ErrorMessage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await ClearLocalStorage();

            //Products = await ProductService.GetItems();
            Products = await ManageProductsLocalStorageService.GetCollection();

            //var shoppingCartItems = await ShoppingCartService.GetItems(HardCoded.UserId);
            var shoppingCartItems = await ManageCartItemsLocalStorageService.GetCollection();

            var totalQty = shoppingCartItems.Sum(i => i.Qty);

            ShoppingCartService.RaiseEventOnShoppingCartChanded(totalQty);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    protected IOrderedEnumerable<IGrouping<int, ProductDto>> GetGroupedProductsByCategory()
        => from product in Products
           group product by product.CategoryId into prodByCatGroup
           orderby prodByCatGroup.Key
           select prodByCatGroup;

    protected string GetCategoryName(IGrouping<int, ProductDto> groupedProductDto)
        => groupedProductDto.First(pg => pg.CategoryId == groupedProductDto.Key).CategoryName;

    private async Task ClearLocalStorage()
    {
        await ManageProductsLocalStorageService.RemoveCollection();
        await ManageCartItemsLocalStorageService.RemoveCollection();
    }

}
