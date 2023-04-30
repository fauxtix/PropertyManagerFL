using Microsoft.AspNetCore.Mvc;
using PropertyManagerFL.Application.ViewModels.GeoApi.CodigosPostais;
using PropertyManagerFL.Application.ViewModels.GeoApi.Municipios;
using static SkiaSharp.HarfBuzz.SKShaper;

namespace PropertyManagerFL.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class GeoApiController : ControllerBase
    {
        private string _apiUri;
        private readonly HttpClient _httpClient;
        private readonly ILogger<GeoApiController> _logger;

        public GeoApiController(HttpClient httpClient, ILogger<GeoApiController> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiUri = "https://json.geoapi.pt";
        }

        /// <summary>
        /// Geo Api by postal code 
        /// </summary>
        /// <param name="codpst"></param>
        /// <param name="subcodpst"></param>
        /// <returns></returns>
        [HttpGet("GetDataByFullPostalCode/{codpst}/{subcodpst}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetDataByFullPostalCode(string codpst, string subcodpst)
        {
            var endpoint = $"{_apiUri}/cp/{codpst}-{subcodpst}";

            var location = GetControllerActionNames();

            try
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(endpoint),
                };

                GeoApi_CP7 addresses = new();

                using (var response = await _httpClient.SendAsync(request))
                {
                    var success  = response.IsSuccessStatusCode; //.EnsureSuccessStatusCode();
                    if(success)
                    {
                        var result = await response.Content.ReadFromJsonAsync<GeoApi_CP7>();
                        if (result is not null)
                            return Ok(result);
                    }
                    else
                    {
                        return NotFound("Código postal sem dados...");
                    }
                }

                return BadRequest("Parâmetros passados são inválidos");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }

        [HttpGet("GetDataBySinglePostalCode/{codpst}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetDataBySinglePostalCode(string codpst)
        {
            var endpoint = $"{_apiUri}/cp/{codpst}";

            var location = GetControllerActionNames();

            try
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(endpoint),
                };

                GeoApi_CP4 addresses = new();

                using (var response = await _httpClient.SendAsync(request))
                {
                    var status = response.EnsureSuccessStatusCode();
                    var result = await response.Content.ReadFromJsonAsync<GeoApi_CP4>();
                    if (result is not null)
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

        [HttpGet("GetMunicipios")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetMunicipios()
        {
            var endpoint = $"{_apiUri}/municipios";
            var location = GetControllerActionNames();
            try
            {
                using var httpResponse = await _httpClient.GetAsync(endpoint);
                if (!httpResponse.IsSuccessStatusCode)
                    throw new Exception("Oops... Something went wrong");

                var municipios = await httpResponse.Content.ReadFromJsonAsync<List<string>>();
                if (municipios is not null)
                    return Ok(municipios);
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return InternalError($"{location}: {ex.Message} - {ex.InnerException}");
            }
        }

        [HttpGet("GetListaMunicipios")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetListaMunicipios(string municipio)
        {
            var endpoint = $"{_apiUri}/municipios/{municipio}";
            var location = GetControllerActionNames();
            try
            {
                using var httpResponse = await _httpClient.GetAsync(endpoint);
                if (!httpResponse.IsSuccessStatusCode)
                    throw new Exception("Oops... Something went wrong");

                var listaMunicipios = await httpResponse.Content.ReadFromJsonAsync<ListaMunicipios>();
                if (listaMunicipios is not null)
                    return Ok(listaMunicipios);
                else
                    return NotFound();
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
