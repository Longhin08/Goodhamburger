using GoodHamburger.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class MenuController : ControllerBase
{
    private readonly MenuService _menuService;

    public MenuController(MenuService menuService) => _menuService = menuService;

    /// <summary>Returns all items available on the menu.</summary>
    [HttpGet]
    public IActionResult GetMenu() => Ok(_menuService.GetMenu());
}
