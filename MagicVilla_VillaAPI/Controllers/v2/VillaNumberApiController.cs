using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers.v2;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("2.0")]
[ApiController]
public class VillaNumberApiController : ControllerBase
{
    private IVillaNumberRepository _dbVillaNumber;
    private IVillaRepository _dbVilla;
    private IMapper _mapper;
    private ApiResponse _response;

    public VillaNumberApiController(IVillaNumberRepository dbVillaNumber, IVillaRepository dbVilla, IMapper mapper)
    {
        _dbVillaNumber = dbVillaNumber;
        _dbVilla = dbVilla;
        _mapper = mapper;
        _response = new ApiResponse();
    }
    
    [HttpGet]
    public IEnumerable<string> GetTestVersioning()
    {
        return new string[] {"value1", "value2"};
    }
}