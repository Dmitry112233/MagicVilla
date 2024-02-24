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
    
    public VillaApiController(IVillaRepository dbVilla, IMapper mapper)
    {
        _dbVilla = dbVilla;
        _mapper = mapper;
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
    {
        IEnumerable<Villa?> villaList = await _dbVilla.GetAllAsync();
        return Ok(_mapper.Map<List<VillaDTO>>(villaList));
    }
    
    [HttpGet("{id:int}", Name = "GetVilla")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VillaDTO>> GetVilla(int id)
    {
        if (id == 0)
        {
            return BadRequest();
        }
        
        var villa = await _dbVilla.GetAsync(x => x.Id == id);
        
        if (villa == null)
            return NotFound();
        
        return Ok(_mapper.Map<VillaDTO>(villa));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody]VillaCreateDTO? createDto)
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

        Villa? model = _mapper.Map<Villa>(createDto);

        await _dbVilla.CreateAsync(model);
        
        return CreatedAtRoute("GetVilla", new {id = model.Id}, model);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteVilla(int id)
    {
        if (id == 0)
        {
            return BadRequest();
        }
        
        var villa = await _dbVilla.GetAsync(x => x.Id == id);
        
        if (villa == null)
            return NotFound();

        await _dbVilla.RemoveAsync(villa);
        
        return NoContent();
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDTO? updateDto)
    {
        if (updateDto == null || id != updateDto.Id)
        {
            return BadRequest();
        }
        
        Villa? model = _mapper.Map<Villa>(updateDto);

        await _dbVilla.UpdateAsync(model);
        
        return NoContent();
    }
    
    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO>? patchDto)
    {
        if (patchDto == null || id == 0)
        {
            return BadRequest();
        }
        
        var villa = await _dbVilla.GetAsync(x => x.Id == id, false);
        
        VillaUpdateDTO villaDto = _mapper.Map<VillaUpdateDTO>(villa);
        
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