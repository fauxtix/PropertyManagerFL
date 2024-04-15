using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.ViewModels;

namespace PropertyManagerFL.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesFromPostalCodeController : ControllerBase
    {
        private string _apiUri;
        private string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly ILogger<AddressesFromPostalCodeController> _logger;
        public AddressesFromPostalCodeController(HttpClient http, ILogger<AddressesFromPostalCodeController> logger)
        {
            _httpClient = http;
            _logger = logger;
        }

        [HttpGet("GetAddresses/{codpst}/{subcodpst}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetAddresses(string codpst, string subcodpst)
        {
            _apiUri = "https://api.duminio.com/ptcp/v2/";
            _apiKey = "ptapi63a1d523734633.94842924";
            var endpoint = $"{_apiUri}/{_apiKey}/{codpst}{subcodpst}";

            var location = GetControllerActionNames();

            try
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(endpoint),
                };

                using (var response = await _httpClient.SendAsync(request))
                {
                    var status = response.EnsureSuccessStatusCode();
                    var result = await response.Content.ReadFromJsonAsync<List<AddressVM>>();
                    if (result!.Any())
                        return Ok(result);
                    else
                        return NotFound();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }

        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $"{controller} - {action}";
        }

        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Algo de errado ocorreu. Contacte o Administrador");
        }
    }
}
