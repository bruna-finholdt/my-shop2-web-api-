﻿using Minishop.Domain.Entity;
using System.Collections.Generic;

namespace Minishop.Domain.DTO
{
    public class SuppliersCompletoResponse : SuppliersResponse
    {
        public SuppliersCompletoResponse(Supplier supplier, ICollection<Product> products)
           : base(supplier)
        {
            Cnpj = supplier.Cnpj;

            Products = products.Select(product => new ProductsResponse(product)).ToList();

            //Products = new List<ProductsResponse>();

            //foreach (Product product in products)
            //{
            //    Products.Add(new ProductsResponse(product));
            //}
        }

        public string Cnpj { get; private set; }
        public List<ProductsResponse> Products { get; private set; }
    }
}