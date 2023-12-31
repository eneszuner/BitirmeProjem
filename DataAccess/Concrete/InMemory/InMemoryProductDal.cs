﻿using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete.InMemory
{
    public class InMemoryProductDal : IProductDal
    {
        List<Product> _products;//Bu class için global değişken
        public InMemoryProductDal()//ctor yazınca 
        {
            //Oracle,SqlServer,Postgres,MongoDb den geiyormuş gibi simule ediyoruz
            _products = new List<Product> 
            {
                new Product{ProductId=1,CategoryId=1,ProductName="Bardak",UnitPrice=15,UnitsInStock=15},
                new Product{ProductId=2,CategoryId=1,ProductName="Kamera",UnitPrice=500,UnitsInStock=3},
                new Product{ProductId=3,CategoryId=2,ProductName="Telefon",UnitPrice=1500,UnitsInStock=2},
                new Product{ProductId=4,CategoryId=2,ProductName="Klavye",UnitPrice=150,UnitsInStock=65},
                new Product{ProductId=5,CategoryId=2,ProductName="Fare",UnitPrice=85,UnitsInStock=1}
            };
        }
        public void Add(Product product)
        {
            _products.Add(product);
        }

        public void Delete(Product product)
        {
            //Çalışmaz bu
            //_products.Remove(product);
            //_products.Remove(product); refernas numarsı olmadığı için silmez Id göndersen bile silmez

            //LINQ - Language Integrated Query kullanmadan
            //Product productToDelete=null;
            //foreach (var p in _products)
            //{
            //    if (product.ProductId==p.ProductId)
            //    {
            //        productToDelete = p;
            //    }
            //}


            //Bu kod foreach'ı yapıyor
            //=> Lambda
            Product productToDelete = _products.SingleOrDefault(p=>p.ProductId==product.ProductId);
            _products.Remove(productToDelete);
        }

        public Product Get(Expression<Func<Product, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public List<Product> GetAll()
        {
            return _products;//veri tabanını olduğu gibi döndürüyorum çünkü tümünü döndürüyorum 
        }

        public List<Product> GetAll(Expression<Func<Product, bool>> filter = null)
        {
            throw new NotImplementedException();
        }

        public List<Product> GetAllByCategory(int categoryId)
        {
            return _products.Where(p=>p.CategoryId==categoryId).ToList();
            //Where Koşulu şunu yapar içindeki şartları uyan bütün elemanları yeni bir liste haline getirip
            //onu döndürür && koyarak istediğin kadar koşul ekliye bilirsin
        }

        public List<ProductDetailDto> GetProductDetails()
        {
            throw new NotImplementedException();
        }

        public void Update(Product product)
        {
            //Güncellemede de aynı durum 
            //Gönderdiğim ürün id'sine sahip olan listedeki ürünü bul demek
            Product productToUpdate = _products.SingleOrDefault(p => p.ProductId == product.ProductId);
            productToUpdate.ProductName = product.ProductName;
            productToUpdate.CategoryId=product.CategoryId;
            productToUpdate.UnitPrice = product.UnitPrice;
            productToUpdate.UnitsInStock = product.UnitsInStock;
        }
    }
}
