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
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetVillaNumbers()
    {
        try
        {
            var villaNumbersList = await _dbVillaNumber.GetAllAsync();
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = _mapper.Map<List<VillaNumberDto>>(villaNumbersList);
            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return _response;
    }

    [HttpGet("{id:int}", Name = "GetVillaNumber")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> GetVillaNumber(int id)
    {
        try
        {
            if (id == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }

            var villaNumber = await _dbVillaNumber.GetAsync(x => x.VillaNo == id);

            if (villaNumber == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                return NotFound(_response);
            }

            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = _mapper.Map<VillaNumberDto>(villaNumber);
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
    public async Task<ActionResult<ApiResponse>> CreateVillaNumber([FromBody] VillaNumberDto createDto)
    {
        try
        {
            if (await _dbVillaNumber.GetAsync(x => x.VillaNo == createDto.VillaNo) != null)
            {
                ModelState.AddModelError("CustomError", "Villa number already Exist!");
                return BadRequest(ModelState);
            }

            if (createDto == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("Passing object is nul");
                return BadRequest(_response);
            }
            
            if (await _dbVilla.GetAsync(x => x.Id == createDto.VillaId) == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("villa id is not exist");
                return BadRequest(_response);
            }

            var villaNumber = _mapper.Map<VillaNumber>(createDto);
            await _dbVillaNumber.CreateAsync(villaNumber);

            _response.StatusCode = HttpStatusCode.Created;
            _response.Result = _mapper.Map<VillaNumberDto>(villaNumber);

            return CreatedAtRoute("GetVillaNumber", new { id = villaNumber.VillaNo }, _response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return _response;
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> DeleteVillaNumber(int id)
    {
        try
        {
            var villaNumber = await _dbVillaNumber.GetAsync(x => x.VillaNo == id);

            if (villaNumber == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                return NotFound(_response);
            }

            await _dbVillaNumber.RemoveAsync(villaNumber);

            _response.StatusCode = HttpStatusCode.NoContent;
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> UpdateVilla(int id, [FromBody] VillaNumberDto updatedDto)
    {
        try
        {
            var villaNumber = await _dbVillaNumber.GetAsync(x => x.VillaNo == id, false);

            if (villaNumber == null || id != updatedDto.VillaNo)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Some problems with villa number id");
                return BadRequest(_response);
            }
            if (await _dbVilla.GetAsync(x => x.Id == updatedDto.VillaId) == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("villa id is not exist");
                return BadRequest(_response);
            }

            var villaNumberToUpdate = _mapper.Map<VillaNumber>(updatedDto);
            await _dbVillaNumber.UpdateAsync(villaNumberToUpdate);

            _response.StatusCode = HttpStatusCode.NoContent;

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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> UpdatePartialVillaNumber(int id,
        JsonPatchDocument<VillaNumberUpdateDto>? patchDto)
    {
        try
        {
            if (patchDto == null || id == 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }

            var villaNumber = await _dbVillaNumber.GetAsync(x => x.VillaNo == id, false);

            if (villaNumber == null)
            {
                return BadRequest();
            }

            VillaNumberUpdateDto villaNumberDto = _mapper.Map<VillaNumberUpdateDto>(villaNumber);

            patchDto.ApplyTo(villaNumberDto, ModelState);
            
            if (await _dbVilla.GetAsync(x => x.Id == villaNumberDto.VillaId) == null)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("VillaId is not exist");
                return BadRequest(_response);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            VillaNumber model = _mapper.Map<VillaNumber>(villaNumberDto);

            await _dbVillaNumber.UpdateAsync(model);

            _response.StatusCode = HttpStatusCode.NoContent;

            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string>() { e.ToString() };
        }

        return _response;
    }
}