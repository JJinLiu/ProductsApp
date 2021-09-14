using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductsApplication.Controllers.Dto;
using ProductsApplication.Models;
using ProductsApplication.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsApplication.Controllers.Apis
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ProductContext _context;
        private readonly IProductServices _productServices;
        private readonly IMapper _mapper;

        public ProductController(
            ILogger<ProductController> logger, 
            ProductContext context, 
            IProductServices productServices,
            IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _productServices = productServices;
            _mapper = mapper;
        }

        /// <summary>
        /// Get product by Id
        /// </summary>
        /// <param name="productId">The unique identifier for a product</param>
        /// <returns>The product dto object in the response body</returns>
        [HttpGet("Get/{productId}")]
        public IActionResult Get([FromRoute] long productId)
        {
            try
            {
                var product = _productServices.GetProduct(productId);
                var result = _mapper.Map<ProductDto>(product);
                return Ok(result);
            }
            catch (ObjectNotFoundException)
            {
                return NotFound($"Product was not found by Id: {productId}.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Get product by product id: {productId} failed. {ex.Message}");
                return new ObjectResult($"Get product by product id: {productId} failed. {ex.Message}") { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        /// <summary>
        /// Mark a product as deleted
        /// </summary>
        /// <param name="productId">The unique identifier for a product</param>
        [HttpDelete("Delete/{productId}")]
        public async Task<IActionResult> Delete([FromRoute] long productId)
        {
            try
            {
                await _productServices.DeleleProductAsync(productId);
                return Ok();
            }
            catch (ObjectNotFoundException)
            {
                return NotFound($"Product was not found by Id: {productId}.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Delete product by product id: {productId} failed. {ex.Message}");
                return new ObjectResult($"Delete product by product id: {productId} failed. {ex.Message}") { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        /// <summary>
        /// Create a new product record in database
        /// </summary>
        /// <param name="productDto">A product data transfer model with valid infomation</param>
        [HttpPost("GetProduct/Create")]
        public async Task<IActionResult> Create([FromBody] ProductDto productDto)
        {
            try
            {
                var product = _mapper.Map<Product>(productDto);
                await _productServices.AddProductAsync(product);
                return Ok();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Product name is invalid"))
                    return BadRequest($"Invalid request body. {ex.Message}");

                _logger.LogError($"Create product with product name: {productDto.Name} failed. {ex.Message}");
                return new ObjectResult($"Create product with product name: {productDto.Name} failed. {ex.Message}") { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        /// <summary>
        /// Update an existing product record in database
        /// </summary>
        /// <param name="productId">The unique identifier for a product</param>
        /// <param name="productDto">A product data transfer model with valid infomation</param>
        [HttpPost("GetProduct/Edit/{productId}")]
        public async Task<IActionResult> Edit([FromRoute] long productId, [FromBody] ProductDto productDto)
        {
            try
            {
                var product = _mapper.Map<Product>(productDto);
                await _productServices.UpdateProductAsync(productId, product);
                return Ok();
            }
            catch (ObjectNotFoundException)
            {
                return NotFound($"Product was not found by Id: {productId}.");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Product name is invalid"))
                    return BadRequest($"Invalid request body. {ex.Message}");

                _logger.LogError($"Editing product with product name: {productDto.Name} failed. {ex.Message}");
                return new ObjectResult($"Editing product with product name: {productDto.Name} failed. {ex.Message}") { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }


        /// <summary>
        /// List all the un-deleted products contain a product name in the database with pagination
        /// </summary>
        /// <param name="productName">The product name used for searching</param>
        /// <param name="pagedInputDto">Pagination requirements</param>
        /// <returns>A list of all the un-deleted products contain a product name in the response body</returns>
        [HttpGet("Search/{productName}")]
        public IActionResult Search([FromRoute] string productName, [FromQuery] PagedInputDto pagedInputDto)
        {
            try 
            {
                var productsList = _productServices.SearchProductsByName(productName)
                    .Skip((pagedInputDto.PageNumber - 1) * pagedInputDto.PageSize)
                    .Take(pagedInputDto.PageSize)
                    .ToList();
                var result = _mapper.Map<List<ProductDto>>(productsList);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Searching products by product name: {productName} failed. {ex.Message}");
                return new ObjectResult($"Searching products by product name: {productName} failed {ex.Message}") { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }
    }
}