﻿using Minishop.Domain.Entity;
using System.Collections.Generic;

namespace Minishop.Domain.DTO
{
    public class SuppliersCompletoResponse : SuppliersResponse
    {
        public SuppliersCompletoResponse(Supplier supplier)
           : base(supplier)
        {
            

            Products = supplier.Products.Select(product => new ProductsResponse(product)).ToList();

            //Products = new List<ProductsResponse>();

            //foreach (Product product in products)
            //{
            //    Products.Add(new ProductsResponse(product));
            //}
            Cnpj = supplier.Cnpj;
        }

        
        public List<ProductsResponse> Products { get; private set; }
        public string Cnpj { get; private set; }
    }
}
