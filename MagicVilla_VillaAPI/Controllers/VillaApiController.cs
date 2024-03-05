using System.Net;
using AutoMapper;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers;

[Route("api/[controller]")]
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetVillas()
    {
        try
        {
            IEnumerable<Villa?> villaList = await _dbVilla.GetAllAsync();
            _response.StatusCode = HttpStatusCode.OK;
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> GetVilla(int id)
    {
        try
        {
            if (id == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            var villa = await _dbVilla.GetAsync(x => x.Id == id);

            if (villa == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_response);
            }

            _response.StatusCode = HttpStatusCode.OK;
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
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> CreateVilla([FromBody] VillaCreateDto createDto)
    {
        try
        {
            if (await _dbVilla.GetAsync(x => x.Name.ToLower() == createDto.Name) != null)
            {
                ModelState.AddModelError("CustomError", "Villa already Exist!");
                return BadRequest(ModelState);
            }

            if (createDto == null)
            {
                return BadRequest(createDto);
            }

            Villa? villa = _mapper.Map<Villa>(createDto);

            await _dbVilla.CreateAsync(villa);

            _response.StatusCode = HttpStatusCode.Created;
            _response.Result = _mapper.Map<VillaDto>(villa);

            return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return _response;
    }

    [HttpDelete("{id:int}")]
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

            if (villa == null)
                return NotFound();

            await _dbVilla.RemoveAsync(villa);

            _response.StatusCode = HttpStatusCode.NoContent;
            _response.IsSuccess = true;

            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return _response;
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDto? updateDto)
    {
        try
        {
            if (updateDto == null || id != updateDto.Id)
            {
                return BadRequest();
            }

            Villa? model = _mapper.Map<Villa>(updateDto);

            await _dbVilla.UpdateAsync(model);

            _response.StatusCode = HttpStatusCode.NoContent;
            _response.IsSuccess = true;

            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
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