using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductSearcherApi.Repositories;
using TheTourGuy.DTO.Request;
using TheTourGuy.Models;
using TheTourGuy.Models.Internal;

namespace ProductSearcherApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ProductRepository _repository;

    public ProductsController(IMapper mapper, ProductRepository repository)
    {
        _mapper = mapper;
        _repository = new ProductRepository(new RabbitMqConfiguration());
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] ProductFilterRequest productFilterRequest )
    {
        var productFilter = _mapper.Map<ProductFilterRequest,ProductFilter>(productFilterRequest);
        var products = await _repository.SearchProducts(productFilter);
        var pagedProducts = products.Skip(productFilterRequest.PageIndex * productFilterRequest.PageSize).Take(productFilterRequest.PageSize);

        return Ok(pagedProducts);
    }
}