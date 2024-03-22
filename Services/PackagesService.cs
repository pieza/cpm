using cpm.DTOs;
using cpm.Utils;
using System.Net.Http.Json;

namespace cpm.Services
{
    internal class PackagesService
    {
        public static async Task<PackageDTO?> Find(string identifier)
        {
            try
            {
                using var client = new HttpClient();
                string? apiUrl = Config.GetProperty("RepositoryUrl");
                if (string.IsNullOrEmpty(apiUrl))
                {
                    throw new Exception("Repository URL not set.");
                }
                var result = await client.GetFromJsonAsync<PackageDTO?>($"{apiUrl}/packages/{identifier}");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Logger.Instance.Error(ex.Message);
            }
            return null;
        }
    }
}
