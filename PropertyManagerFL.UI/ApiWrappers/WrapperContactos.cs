using AutoMapper;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.ViewModels.Contactos;

namespace PropertyManagerFL.UI.ApiWrappers
{
    /// <summary>
    /// Wrapper Contactos Api
    /// </summary>
    public class WrapperContactos : IContactosService
    {
        private readonly IConfiguration _env;
        private readonly ILogger<WrapperContactos> _logger;
        private readonly string? _uri;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        /// <summary>
        /// Construtor wrapperContactos
        /// </summary>
        /// <param name="env"></param>
        /// <param name="logger"></param>
        /// <param name="httpClient"></param>
        /// <param name="mapper"></param>
        public WrapperContactos(IConfiguration env, ILogger<WrapperContactos> logger, HttpClient httpClient, IMapper mapper)
        {
            _env = env;
            _uri = $"{_env["BaseUrl"]}/Contactos";
            _logger = logger;
            _httpClient = httpClient;
            _mapper = mapper;
        }

        /// <summary>
        /// New contact
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        public async Task<bool> InsereContacto(ContactoVM contact)
        {
            try
            {
                var contactToInsert = _mapper.Map<NovoContacto>(contact);

                using (HttpResponseMessage result = await _httpClient.PostAsJsonAsync($"{_uri}/InsereContacto", contactToInsert))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao criar Contacto {exc.Message}");
                return false;
            }
        }

        /// <summary>
        /// Update contact
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contact"></param>
        /// <returns></returns>
        public async Task<bool> AtualizaContacto(int id, ContactoVM contact)
        {
            try
            {
                var contactToUpdate = _mapper.Map<AlteraContacto>(contact);

                using (HttpResponseMessage result = await _httpClient.PutAsJsonAsync($"{_uri}/AlteraContacto/{id}", contactToUpdate))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao atualizar Contacto");
                return false;
            }
        }

        /// <summary>
        /// Delete contact
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> ApagaContacto(int id)
        {
            try
            {
                using (HttpResponseMessage result = await _httpClient.DeleteAsync($"{_uri}/ApagaContacto/{id}"))
                {
                    var success = result.IsSuccessStatusCode;
                    return success;
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Erro ao apagar Contacto)");
                return false;
            }
        }

        /// <summary>
        /// Get all contacts
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ContactoVM>> GetAll()
        {
            try
            {
                var contacts = await _httpClient.GetFromJsonAsync<IEnumerable<ContactoVM>>($"{_uri}/GetContactos");
                return contacts!.ToList();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return Enumerable.Empty<ContactoVM>();
            }
        }

        /// <summary>
        /// Get contact by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ContactoVM> GetContacto_ById(int id)
        {
            try
            {
                var contact = await _httpClient.GetFromJsonAsync<ContactoVM>($"{_uri}/GetContactoById/{id}");
                return contact!;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Erro ao pesquisar API");
                return new ContactoVM();
            }
        }
    }
}
