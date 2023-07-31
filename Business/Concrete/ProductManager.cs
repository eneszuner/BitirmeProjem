using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.CCS;
using Business.Constants;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Performance;
using Core.Aspects.Autofac.Transaction;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Validation;
using Core.Utilities.Business;
using Core.Utilities.Results;
using DataAccess.Abstract;
using DataAccess.Concrete.InMemory;
using Entities.Concrete;
using Entities.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class ProductManager : IProductService
    {
        IProductDal _productDal;
        ICategoryService _categoryService;

        public ProductManager(IProductDal productDal, ICategoryService categoryService)//Ampul'den Generate Constructor'a tıklıyoruz
        {
            _productDal = productDal;
            _categoryService = categoryService;
        }

        //Claim=iddia etmek demek
        [SecuredOperation("product.add,admin")]//product.add veya admin den birisine sahip olması  demektir
        [ValidationAspect(typeof(ProductValidator))]
        [CacheRemoveAspect("IProductService.Get")]
        public IResult Add(Product product)
        {
            //Loglama Kodları çalışıcak
            IResult result = BusinessRules.Run(CheckIfProductNameExists(product.ProductName),
                 CheckIfProductCountOfCategoryCorrect(product.CategoryId),
                 CheckIfCategoryLimitExcede());

            if (result != null)//Kurala uymayan bir durum oluşmuşsa demek 
            {
                return result;
            }
            _productDal.Add(product);
            return new SuccessResult(Messages.ProductAdded);

            /*--------GEREK YOK------------------
             ProductValidator içine yazılmıştır
            İş Kodları yazılır business kodlar buraya yazılır
            validation
            if (product.UnitPrice <= 0)
            {
                return new ErrorResult(Messages.UnitPriceInvalid);
            }
            if (product.ProductName.Length < 2)
            {
                //magic strings
                return new ErrorResult(Messages.ProductNameInvalid);
            }
            ----------------------------------------------------------
            loglama
            cacheremove
            performance
            transaction
            yetkilendirmeler
            */

            //--------------GEREK YOK---------------------------
            //if (CheckIfProductCountOfCategoryCorrect(product.CategoryId).Success)&&CheckIfProductNameExists(product.ProductName).Success bu şekilde de yazılır
            //{
            //    if (CheckIfProductNameExists(product.ProductName).Success)
            //    {

            //    }

            //}
            //return new ErrorResult();
            //---------------------------------------------------------
        }

        [CacheAspect] //key,value
        public IDataResult<List<Product>> GetAll()
        {
            //iş kodları
            //Data Access kısmından referans alındı
            //InMemoryProductDal ınMemoryProductDal = new InMemoryProductDal(); iş sınıfı asla başka sınıfları newlemez
            if (DateTime.Now.Hour == 16)
            {
                return new ErrorDataResult<List<Product>>(Messages.MaintenanceTime);//Generate field
            }
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(),Messages.ProductListed);//Generate field
        }

        public IDataResult<List<Product>> GetAllByCategoryId(int id)
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(p=>p.CategoryId==id),Messages.ProductListed);
        }

        [CacheAspect]
        [PerformanceAspect(5)]
        public IDataResult<Product> GetById(int productId)
        {
            return new SuccessDataResult<Product>(_productDal.Get(p => p.ProductId == productId),Messages.ProductListed);
        }

        public IDataResult<List<Product>> GetByUnitPrice(decimal min, decimal max)
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(p => p.UnitPrice >= min && p.UnitPrice <= max),Messages.ProductListed);
        }

        public IDataResult<List<ProductDetailDto>> GetProductDetails()
        {
            if (DateTime.Now.Hour == 16)
            {
                return new ErrorDataResult<List<ProductDetailDto>>(Messages.MaintenanceTime);//Generate field
            }
            return new SuccessDataResult<List<ProductDetailDto>>(_productDal.GetProductDetails(),Messages.ProductListed); 
        }

        [ValidationAspect(typeof(ProductValidator))]
        //[CacheRemoveAspect("Get")]Bellekteki tüm getleri içerisinde get olan tüm keyleri iptal et demek dolayısıyla sen ürünü güncellemişken her yerdeki cache silersin 
        [CacheRemoveAspect("IProductService.Get")]//IProductService'deki tüm getleri sil demek
        public IResult Update(Product product)
        {
            var result = _productDal.GetAll(p => p.CategoryId == product.CategoryId).Count;
            if (result >= 10)
            {
                return new ErrorResult(Messages.ProductCountOfCategoryError);
            }
            return new SuccessResult(Messages.ProductUpdated);
        }

        private IResult CheckIfProductCountOfCategoryCorrect(int categoryId)//Product product olsa bile olur
        {
            //Bir kategoride en fazla 15 ürün olabilir  
            //Select count(*) from Product where categoryId=1
            //arka planda bunu çalıştırır
            var result = _productDal.GetAll(p => p.CategoryId == categoryId).Count;//bu arka planda bir linq oluşturuyor veritabanına o linq yolluyor
            if (result >= 15)
            {
                return new ErrorResult(Messages.ProductCountOfCategoryError);
            }
            return new SuccessResult();
        }
        private IResult CheckIfProductNameExists(string productName)//Product product olsa bile olur
        {
            //Aynı isimde ürün eklenemez
            var result = _productDal.GetAll(p => p.ProductName == productName).Any();
            if (result)
            {
                return new ErrorResult(Messages.ProductNameAlreadyExists);
            }
            return new SuccessResult();
        }
        /*Eğer bu kuralı categorymanager'a yazarsak bu demekki tek başına bir sevice demektir 
         * ama bu tamameno categoryserivce ni kullanan bir ürünün onu nasıl ele aldığı ile alakalı
         */
        private IResult CheckIfCategoryLimitExcede()
        {
            //Eğer mevcut kategori sayısı 15'i geçtiyse sisteme yeni ürün eklenemez 
            var result = _categoryService.GetAll();
            if (result.Data.Count>15)
            {
                return new ErrorResult(Messages.CategoryLimitExcede);
            }
            return new SuccessResult();
        }

        [TransactionScopeAspect]
        public IResult AddTransactionalTest(Product product)
        {
            _productDal.Update(product);
            _productDal.Add(product);
            return new SuccessResult(Messages.ProductUpdated);
        }

        public IDataResult<List<Product>> GetByCategoryId(int categoryId)
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(p => p.CategoryId == categoryId), Messages.ProductListed);
        }
    }
}
