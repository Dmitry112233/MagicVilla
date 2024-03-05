using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MagicVilla_Web.Controllers;

public class VillaController : Controller
{
    private readonly IVillaService _villaService;
    private readonly IMapper _mapper;

    public VillaController(IVillaService villaService, IMapper mapper)
    {
        _villaService = villaService;
        _mapper = mapper;
    }

    public async Task<IActionResult> CreateVilla()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateVilla(VillaCreateDto model)
    {
        if (ModelState.IsValid)
        {
            var response = await _villaService.CreateAsync<ApiResponse>(model);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(IndexVilla));
            }
        }

        return View(model);
    }

    public async Task<IActionResult> UpdateVilla(int villaId)
    {
        var response = await _villaService.GetAsync<ApiResponse>(villaId);
        if (response != null && response.IsSuccess)
        {
            VillaDto model = JsonConvert.DeserializeObject<VillaDto>(Convert.ToString(response.Result));
            return View(_mapper.Map<VillaUpdateDto>(model));
        }

        return NotFound();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateVilla(VillaUpdateDto model)
    {
        if (ModelState.IsValid)
        {
            var response = await _villaService.UpdateAsync<ApiResponse>(model);
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(IndexVilla));
            }
        }

        return View(model);
    }

    public async Task<IActionResult> DeleteVilla(int villaId)
    {
        var response = await _villaService.GetAsync<ApiResponse>(villaId);
        if (response != null && response.IsSuccess)
        {
            VillaDto model = JsonConvert.DeserializeObject<VillaDto>(Convert.ToString(response.Result));
            return View(model);
        }

        return NotFound();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteVilla(VillaDto model)
    {
        var response = await _villaService.DeleteAsync<ApiResponse>(model.Id);
        if (response != null && response.IsSuccess)
        {
            return RedirectToAction(nameof(IndexVilla));
        }

        return View(model);
    }


    public async Task<IActionResult> IndexVilla()
    {
        List<VillaDto> list = new();
        var response = await _villaService.GetAllAsync<ApiResponse>();
        if (response != null)
        {
            list = JsonConvert.DeserializeObject<List<VillaDto>>(Convert.ToString(response.Result));
        }

        return View(list);
    }
}