using Microsoft.AspNetCore.Mvc;
using FutureLife.API.Helpers;
using FutureLife.API.Services;

namespace FutureLife.API.Controllers;

[ApiController]
[Route("exchange-rates")]
public class ExchangeRatesController : ControllerBase
{
    private readonly CurrencyService _currency;

    public ExchangeRatesController(CurrencyService currency)
    {
        _currency = currency;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var rates = await _currency.GetAllRatesAsync();
        return Ok(ApiResponse.Success(rates));
    }
}
