﻿using Microsoft.AspNetCore.Components;
using ShopOnline.Models.Dtos;
using ShopOnlineMudBlazor.Web.Services.Contracts;

namespace ShopOnlineMudBlazor.Web.Pages.Products
{
    public class ProductsBase : ComponentBase
    {
        [Inject]
        public IProductService ProductService { get; set; }

        [Inject]
        public IShoppingCartService ShoppingCartService { get; set; }

        public IEnumerable<ProductDto> Products { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public string ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Products = await ProductService.GetItems();

                var shoppingCartItems = await ShoppingCartService.GetItems(HardCoded.UserId);

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
    }
}