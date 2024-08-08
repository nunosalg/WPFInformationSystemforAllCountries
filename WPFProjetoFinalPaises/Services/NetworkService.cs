using System.Net;
using WPFProjetoFinalPaises.Models;

namespace WPFProjetoFinalPaises.Services
{
    public class NetworkService
    {
        /// <summary>
        /// Check if there's internet connection
        /// </summary>
        /// <returns></returns>
        public Response CheckConnection()
        {
            var client = new WebClient();

            try
            {
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return new Response
                    {
                        IsSucess = true
                    };
                }
            }
            catch
            {

                return new Response
                {
                    IsSucess = false,
                    Message = "Configure your internet connection",
                };
            }
        }
    }
}
