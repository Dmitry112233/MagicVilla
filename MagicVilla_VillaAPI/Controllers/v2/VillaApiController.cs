using System.Net;
using System.Text.Json;
using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers.v2;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("2.0")]
[ApiController]
public class VillaApiController : ControllerBase
{
    private readonly IVillaRepository _dbVilla;
    private readonly IMapper _mapper;
    private readonly ApiResponse _response;

    public VillaApiController(IVillaRepository dbVilla, IMapper mapper)
    {
        _dbVilla = dbVilla;
        _mapper = mapper;
        this._response = new();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    //[ResponseCache(CacheProfileName = "Default30")]
    public async Task<ActionResult<ApiResponse>> GetVillas([FromQuery(Name = "filterOccupancy")]int? occupancy, [FromQuery(Name = "searchName")]string? search, int pageSize = 0, int pageNumber = 1)
    {
        try
        {
            IEnumerable<Villa> villaList;
            
            if (occupancy > 0)
            {
                villaList = await _dbVilla.GetAllAsync(u => u.Occupancy == occupancy, pageSize:pageSize, pageNumber:pageNumber);
            }
            else
            {
                villaList = await _dbVilla.GetAllAsync(pageSize:pageSize, pageNumber:pageNumber);
            }

            if (!string.IsNullOrEmpty(search))
            {
                villaList = villaList.Where(u => u.Name.ToLower().Contains(search));
            }
            var pagination = new Pagination { PageNumber = pageNumber, PageSize = pageSize };
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));
            _response.Result = _mapper.Map<List<VillaDto>>(villaList);
            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return _response;
    }

    [HttpGet("{id:int}", Name = "GetVilla")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> GetVilla(int id)
    {
        try
        {
            if (id == 0)
            {
                return BadRequest(_response);
            }

            var villa = await _dbVilla.GetAsync(x => x.Id == id);

            if (villa == null)
            {
                return NotFound(_response);
            }
            
            _response.Result = _mapper.Map<VillaDto>(villa);

            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return _response;
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> CreateVilla([FromForm] VillaCreateDto createDto)
    {
        try
        {
            if (await _dbVilla.GetAsync(x => x.Name.ToLower() == createDto.Name) != null)
            {
                ModelState.AddModelError("ErrorMessages", "Villa already Exist!");
                return BadRequest(ModelState);
            }

            if (createDto == null)
            {
                return BadRequest(createDto);
            }

            Villa? villa = _mapper.Map<Villa>(createDto);

            await _dbVilla.CreateAsync(villa);

            if (createDto.Image != null)
            {
                string fileName = villa.Id + Path.GetExtension(createDto.Image.FileName);
                string filePath = $"wwwroot{Path.DirectorySeparatorChar}ProductImages{Path.DirectorySeparatorChar}" + fileName;

                var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                FileInfo file = new FileInfo(directoryLocation);

                if (file.Exists)
                {
                    file.Delete();
                }

                using (var fileStream = new FileStream(directoryLocation, FileMode.Create))
                {
                    createDto.Image.CopyTo(fileStream);
                }

                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                villa.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                villa.ImageLocalPath = filePath;
            }
            else
            {
                villa.ImageUrl = "https://placehold.co/600x400";
            }

            await _dbVilla.UpdateAsync(villa);
            _response.Result = _mapper.Map<VillaDto>(villa);
            _response.StatusCode = HttpStatusCode.Created;
            return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response);
        }
        catch (Exception e)
        {
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return _response;
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> DeleteVilla(int id)
    {
        try
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var villa = await _dbVilla.GetAsync(x => x.Id == id);
            
            if (!string.IsNullOrEmpty(villa.ImageLocalPath))
            {
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), villa.ImageLocalPath);
                    
                FileInfo file = new FileInfo(oldPath);

                if (file.Exists)
                {
                    file.Delete();
                }
            }
            
            await _dbVilla.RemoveAsync(villa);

            _response.IsSuccess = true;

            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return _response;
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> UpdateVilla(int id, [FromForm] VillaUpdateDto? updateDto)
    {
        try
        {
            if (updateDto == null || id != updateDto.Id)
            {
                return BadRequest();
            }

            Villa? model = _mapper.Map<Villa>(updateDto);

            if (updateDto.Image != null)
            {
                if (!string.IsNullOrEmpty(model.ImageLocalPath))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), model.ImageLocalPath);
                    
                    FileInfo file = new FileInfo(oldPath);

                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }
                
                string fileName = model.Id + Path.GetExtension(updateDto.Image.FileName);
                string filePath = $"wwwroot{Path.DirectorySeparatorChar}ProductImages{Path.DirectorySeparatorChar}" + fileName;

                var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                using (var fileStream = new FileStream(directoryLocation, FileMode.Create))
                {
                    updateDto.Image.CopyTo(fileStream);
                }

                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                model.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                model.ImageLocalPath = filePath;
            }
            else
            {
                model.ImageUrl = "https://placehold.co/600x400";
            }
            
            await _dbVilla.UpdateAsync(model);

            _response.IsSuccess = true;

            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return _response;
    }

    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto>? patchDto)
    {
        if (patchDto == null || id == 0)
        {
            return BadRequest();
        }

        var villa = await _dbVilla.GetAsync(x => x.Id == id, false);

        VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa);

        if (villa == null)
        {
            return BadRequest();
        }

        patchDto.ApplyTo(villaDto, ModelState);

        Villa? model = _mapper.Map<Villa>(villaDto);

        await _dbVilla.UpdateAsync(model);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return NoContent();
    }
}