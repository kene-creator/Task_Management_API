using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Task_Management.Interface;

[ApiController]
[Route("api/[controller]")]
public class BanksController : ControllerBase
{
    private readonly ILoggerManager _logger;
    private readonly FlutterwaveService _flutterwaveService;

    public BanksController(FlutterwaveService flutterwaveService, ILoggerManager logger)
    {
        _flutterwaveService = flutterwaveService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetBanks()
    {
        try
        {
            _logger.LogInfo("Attempting to retrieve bank list from Flutterwave API.");

            var bankList = await _flutterwaveService.GetBankListAsync();

            _logger.LogInfo("Successfully retrieved bank list from Flutterwave API.");
            return Ok(bankList);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong inside GetBanks action: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }
}
