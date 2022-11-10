using Microsoft.AspNetCore.Components;
using ShopOnline.Models.Dtos;
using ShopOnlineMudBlazor.Web.Services.Contracts;

namespace ShopOnlineMudBlazor.Web.Pages.Checkout;

public class CheckoutBase : ComponentBase
{
    [Inject]
    public IShoppingCartService ShoppingCartService { get; set; }

    public List<CartItemDto> ShoppingCartItems { get; set; }

    public string ErrorMessage { get; set; }

    protected string TotalPrice { get; set; }

    protected double TotalPriceQuantity { get; set; }

    protected int TotalQuantity { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            ShoppingCartItems = await ShoppingCartService.GetItems(HardCoded.UserId);

            CalculateCartSummaryTotals();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    private void CalculateCartSummaryTotals()
    {
        SetTotalPrice();
        SetTotalQuantity();
    }

    private void SetTotalPrice()
    {
        TotalPrice = ShoppingCartItems.Sum(lbda => lbda.TotalPrice).ToString("C");
    }

    private void SetTotalQuantity()
    {
        TotalQuantity = ShoppingCartItems.Sum(lbda => lbda.Qty);
    }
}
