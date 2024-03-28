using System.Diagnostics;
using AutoMapper;
using MagicVilla_Utility;
using Microsoft.AspNetCore.Mvc;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;
using Newtonsoft.Json;

namespace MagicVilla_Web.Controllers;

public class HomeController : Controller
{
    private readonly IVillaService _villaService;
    private readonly IMapper _mapper;

    public HomeController(IVillaService villaService, IMapper mapper)
    {
        _villaService = villaService;
        _mapper = mapper;
    }
    
    public async Task<IActionResult> Index()
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