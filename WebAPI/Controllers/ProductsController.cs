using Business.Abstract;
using Business.Concrete;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]//Route= Bu isteği yaparken bu insanlar bize nasıl ulaşsın 
    [ApiController]//ATTRIBUTE 
    public class ProductsController : ControllerBase//Contorller'ın controller olabilmesi için ControllerBase'ten inherit edilmesi ve ApiController 
    {
        //Loosely coupled
        //IoC Container --> Inversion of Control
        IProductService _productService;
        
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }



        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            //Dependency chain

            //Thread.Sleep(2000);
            var result= _productService.GetAll();
            if (result.Success) 
            {
                return Ok(result);
            }
            return BadRequest(result);

        }

        [HttpGet("getbyid")]
        public IActionResult Get(int id)
        {
            var result=_productService.GetById(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result); 
        }

        [HttpGet("getbycategoryid")]
        public IActionResult GetByCategoryId(int categoryId)
        {
            var result = _productService.GetByCategoryId(categoryId);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("getproductdetails")]
        public IActionResult GetProductDetails() 
        { 
            var result=_productService.GetProductDetails();
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost("add")]
        public IActionResult Add(Product product)
        {
            var result = _productService.Add(product);
            if (result.Success)
            {
                return Ok(result);
;           }
            return BadRequest(result);
        }

    }
}
