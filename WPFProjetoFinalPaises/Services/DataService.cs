using System.Data.SQLite;
using System.IO;
using System.Net.Http;
using System.Windows;
using Newtonsoft.Json;
using WPFProjetoFinalPaises.Models;

namespace WPFProjetoFinalPaises.Services
{
    public class DataService
    {
        private SQLiteConnection connection;

        private SQLiteCommand command;

        public DataService()
        {
            // Creates Data folder, if it doesn't exist
            if (!Directory.Exists("Data"))
            {
                Directory.CreateDirectory("Data");
            }

            var path = @"Data\Countries.sqlite";
            // Creates the database and countries' table 
            try
            {
                connection = new SQLiteConnection("Data Source =" + path);
                connection.Open();

                string sqlcommand = "CREATE TABLE IF NOT EXISTS Countries(CountriesInfo text)";

                command = new SQLiteCommand(sqlcommand, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Erro ao criar tabela");
            }
        }

        /// <summary>
        /// Saves the countries' data in the database
        /// </summary>
        /// <param name="result"></param>
        public void SavaData(string result)
        {
            try
            {
                string sql = string.Format("INSERT INTO Countries (CountriesInfo) VALUES (@CountriesInfo)", result);

                using (command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@CountriesInfo", result);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Erro ao gravar dados");
            }
        }

        /// <summary>
        /// Gets the countries' data from the database
        /// </summary>
        /// <returns></returns>
        public List<Country> GetData()
        {
            List<Country> countries = new List<Country>();

            try
            {
                string sql = "SELECT CountriesInfo FROM Countries";

                command = new SQLiteCommand(sql, connection);

                SQLiteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    countries = JsonConvert.DeserializeObject<List<Country>>((string)reader["CountriesInfo"]);
                }

                connection.Close();

                return countries;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Erro a carregar países da base de dados");
                return null;
            }
        }

        /// <summary>
        /// Deletes the data in the database
        /// </summary>
        public void DeleteData()
        {
            try
            {
                string sql = "DELETE FROM Countries";

                command = new SQLiteCommand(sql, connection);

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Erro a apagar dados da base de dados");
            }
        }

        /// <summary>
        /// Saves countries' flags in a local folder
        /// </summary>
        /// <param name="countries"></param>
        /// <param name="client"></param>
        public async void SaveFlags(List<Country> countries, HttpClient client)
        {
            // Creates folder ImagesFlags, if it doesn't exist
            if (!Directory.Exists("ImagesFlags"))
            {
                Directory.CreateDirectory("ImagesFlags");
            }

            var pathImages = @"ImagesFlags";

            try
            {
                SaveFlagNull(pathImages, client);

                foreach (var country in countries)
                {
                    if (country.Flags != null && !String.IsNullOrEmpty(country.Flags.Png))
                    {
                        // Gets the link for the country's flag
                        string flagUrl = country.Flags.Png;
                        // Specify the country's name for the flag file
                        string flagName = $"{country.Name.Common}.png";
                        // Path for the local folder + flags' file name
                        string flagFilePath = Path.Combine(pathImages, flagName);

                        var flagResponse = await client.GetAsync(flagUrl);

                        if (flagResponse.IsSuccessStatusCode)
                        {
                            var flagData = await flagResponse.Content.ReadAsByteArrayAsync();
                            File.WriteAllBytes(flagFilePath, flagData);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Erro ao gravar bandeiras");
            }
        }

        /// <summary>
        /// Downloads Null flag and saves it in a local folder
        /// </summary>
        /// <param name="pathImages"></param>
        /// <param name="client"></param>
        public async void SaveFlagNull(string pathImages, HttpClient client)
        {
            // Creates a new flag to get the default link
            Flag flagNull = new Flag();
            // Flags' link for download
            string flagNullUrl = flagNull.Png;
            // Flags' file name for saving in the local folder
            string flagNullName = $"Null.png";
            // Path for the local folder + flags' file name
            string flagNullFilePath = Path.Combine(pathImages, flagNullName);

            var flagNullResponse = await client.GetAsync(flagNullUrl);

            if (flagNullResponse.IsSuccessStatusCode)
            {
                var flagNullData = await flagNullResponse.Content.ReadAsByteArrayAsync();
                File.WriteAllBytes(flagNullFilePath, flagNullData);
            }
        }
    }
}
