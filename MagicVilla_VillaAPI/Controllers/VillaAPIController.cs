using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VillaAPIController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<VillaDTO>> GetVillas()
    {
        return Ok(VillaStore.villaList);
    }
    
    [HttpGet("{id:int}", Name = "GetVilla")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<VillaDTO> GetVilla(int id)
    {
        if (id == 0)
        {
            return BadRequest();
        }
        
        var villa = VillaStore.villaList.Find(x => x.Id == id);
        
        if (villa == null)
            return NotFound();
        
        return Ok(villa);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<VillaDTO> CreateVilla([FromBody]VillaDTO vilaaDTO)
    {
        if (VillaStore.villaList.FirstOrDefault(x =>
                string.Equals(x.Name, vilaaDTO.Name, StringComparison.CurrentCultureIgnoreCase)) != null)
        {
            ModelState.AddModelError("CustomError", "Villa already Exist!");
            return BadRequest(ModelState);
        }
        
        if (vilaaDTO == null)
        {
            return BadRequest(vilaaDTO);
        }
        
        if (vilaaDTO.Id > 0)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        vilaaDTO.Id = VillaStore.villaList.MaxBy(x => x.Id).Id + 1;
        VillaStore.villaList.Add(vilaaDTO);

        return CreatedAtRoute("GetVilla", new {id = vilaaDTO.Id}, vilaaDTO);
    }
}