using System;
using static System.Console;
using Packt.Shared;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LinqWithEFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            // FilterAndSort();
            // JoinCategoriesAndProducts();
            GroupJoinCategoriesAndProducts();
        }

        static void FilterAndSort()
        {
            using (var db = new Northwind())
            {
                var query = db.Products
                // query is a DbSet<Product>
                .Where(product => product.UnitPrice < 10M)
                // query is now an IQueryable<Product>
                .OrderByDescending(product => product.UnitPrice)
                .Select(product => new // anonymous type
                {
                    product.ProductID,
                    product.ProductName,
                    product.UnitPrice
                });

                // log query output
                WriteLine(query.ToQueryString());

                WriteLine("Products that cost less than 10M");
                foreach (var item in query)
                {
                    WriteLine($"{item.ProductID}: {item.ProductName} costs {item.UnitPrice}");
                }
                WriteLine();
            }
        }

        static void JoinCategoriesAndProducts()
        {
            using (var db = new Northwind())
            {
                // join every product to its category to return 77 matches
                var queryJoin = db.Categories.Join(
                    inner: db.Products,
                    outerKeySelector: category => category.CategoryID,
                    innerKeySelector: product => product.CategoryID,
                    resultSelector: (c, p) =>
                        new { c.CategoryName, p.ProductName, p.ProductID })
                        .OrderBy(cp => cp.CategoryName);

                // log query output
                WriteLine(queryJoin.ToQueryString());

                foreach (var item in queryJoin)
                {
                    WriteLine($"{item.ProductID}: {item.ProductName} is in {item.CategoryName}.");
                }
            }
        }

        static void GroupJoinCategoriesAndProducts()
        {
            using (var db = new Northwind())
            {
                // group all products by their category to return 8 matches
                var queryGroup = db.Categories.AsEnumerable().GroupJoin(
                    inner: db.Products,
                    outerKeySelector: category => category.CategoryID,
                    innerKeySelector: product => product.CategoryID,
                    resultSelector: (c, matchingProducts) =>
                        new { c.CategoryName, Products = matchingProducts.OrderBy(p => p.ProductName) });

                foreach (var item in queryGroup)
                {
                    WriteLine($"{item.CategoryName} has {item.Products.Count()} products.");

                    foreach (var product in item.Products)
                    {
                        WriteLine($" {product.ProductName}");
                    }
                }
            }
        }
    }
}
