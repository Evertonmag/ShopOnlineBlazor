﻿using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ShopOnline.Models.Dtos;
using ShopOnline.Web.Services.Contracts;

namespace ShopOnline.Web.Pages.ShoppingCart;
public class ShoppingCartBase : ComponentBase
{
    [Inject]
    public IJSRuntime Js { get; set; }

    [Inject]
    public IShoppingCartService ShoppingCartService { get; set; }

    [Inject]
    public IManageCartItemsLocalStorageService ManageCartItemsLocalStorageService { get; set; }

    public List<CartItemDto> ShoppingCartItems { get; set; }

    public string ErrorMessage { get; set; }

    protected string TotalPrice { get; set; }
    protected int TotalQuantity { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            //ShoppingCartItems = await ShoppingCartService.GetItems(HardCoded.UserId);
            ShoppingCartItems = await ManageCartItemsLocalStorageService.GetCollection();

            CartChanged();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    protected async Task DeleteCartItem_Click(int id)
    {
        var cartItemDto = await ShoppingCartService.DeleteItem(id);

        RemoveCartItem(id);

        CartChanged();
    }

    protected async Task UpdateQtyCartItem_Click(int id, int qty)
    {
        try
        {
            if (qty > 0)
            {
                var updateItemDto = new CartItemQtyUpdateDto
                {
                    CartItemId = id,
                    Qty = qty
                };

                var returnedUpdateItemDto = await ShoppingCartService.UpdateQty(updateItemDto);

                await UpdateItemTotalPrice(returnedUpdateItemDto);

                CartChanged();

                await MakeUpdateQtyButtonVisible(id, false);
            }
            else
            {
                var item = ShoppingCartItems.FirstOrDefault(lbda => lbda.Id == id);

                if (item != null)
                {
                    item.Qty = 1;
                    item.TotalPrice = item.Price;
                }
            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    protected async Task UpdateQty_Input(int id)
    {
        await MakeUpdateQtyButtonVisible(id, true);
    }

    private async Task MakeUpdateQtyButtonVisible(int id, bool visible)
    {
        await Js.InvokeVoidAsync("MakeUpdateQtyButtonVisible", id, visible);
    }

    private async Task UpdateItemTotalPrice(CartItemDto cartItemDto)
    {
        var item = GetCartItem(cartItemDto.Id);

        if (item != null)
        {
            item.TotalPrice = cartItemDto.Price * cartItemDto.Qty;
        }

        await ManageCartItemsLocalStorageService.SaveCollection(ShoppingCartItems);
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

    private CartItemDto GetCartItem(int id)
    {
        return ShoppingCartItems.FirstOrDefault(lbda => lbda.Id == id);
    }

    private async Task RemoveCartItem(int id)
    {
        var cartItemDto = GetCartItem(id);

        ShoppingCartItems.Remove(cartItemDto);

        await ManageCartItemsLocalStorageService.SaveCollection(ShoppingCartItems);
    }

    private void CartChanged()
    {
        CalculateCartSummaryTotals();
        ShoppingCartService.RaiseEventOnShoppingCartChanded(TotalQuantity);
    }
}
