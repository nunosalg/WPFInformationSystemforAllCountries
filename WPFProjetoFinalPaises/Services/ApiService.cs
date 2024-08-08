using Newtonsoft.Json;
using System.Net.Http;
using WPFProjetoFinalPaises.Models;

namespace WPFProjetoFinalPaises.Services
{
    public class ApiService
    {
        DataService dataService;

        public async Task<Response> GetCountries(string urlBase, string controller)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);

                var response = await client.GetAsync(controller);

                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSucess = false,
                        Message = result,
                    };
                }

                dataService = new DataService();

                dataService.DeleteData();
                // Saves JSON data in the local database
                dataService.SavaData(result);

                // Converts the JSON data into a List<Country>
                var countries = JsonConvert.DeserializeObject<List<Country>>(result);

                //Downloads countries' flags into a local folder
                dataService.SaveFlags(countries, client);

                return new Response
                {
                    IsSucess = true,
                    Result = countries,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSucess = false,
                    Message = ex.Message,
                };
            }
        }
    }
}
