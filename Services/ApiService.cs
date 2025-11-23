using System.Net.Http.Json;
using GreenGuard.Models;

namespace GreenGuard.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;

        public ApiService()
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7113/api/")
            }; 
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                (message, cert, chain, errors) => true
            };

            _http = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://localhost:7113/api/")
            };
        }

         
        public async Task<bool> RegisterInternalUser(InternalUser user)
        {
            var res = await _http.PostAsJsonAsync("InternalUsers", user);
            return res.IsSuccessStatusCode;
        }

        public async Task<InternalUser?> LoginInternal(string email, string password)
        {
            var req = new LoginRequest { Email = email, Password = password };

            var res = await _http.PostAsJsonAsync("InternalUsers/login", req);

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<InternalUser>();
        }

        // ---------------- EXTERNAL USER ----------------
        public async Task<bool> RegisterExternalUser(ExternalUser user)
        {
            var res = await _http.PostAsJsonAsync("ExternalUsers", user);
            return res.IsSuccessStatusCode;
        }

        public async Task<ExternalUser?> LoginExternal(string email, string password)
        {
            var req = new LoginRequest { Email = email, Password = password };

            var res = await _http.PostAsJsonAsync("ExternalUsers/login", req);

            if (!res.IsSuccessStatusCode)
                return null;

            return await res.Content.ReadFromJsonAsync<ExternalUser>();
        }
        public async Task<AdminDashboardResponse?> GetAdminDashboard()
        {
            return await _http.GetFromJsonAsync<AdminDashboardResponse>("Analytics/dashboard");
        }
        public async Task<bool> AddTree(Tree tree)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("Trees", tree);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
        public async Task<List<Tree>> GetTrees()
        {
            return await _http.GetFromJsonAsync<List<Tree>>("Trees");
        }
        public async Task<List<Tree>> GetAllTrees()
        {
            try
            {
                return await _http.GetFromJsonAsync<List<Tree>>("Trees")
                       ?? new List<Tree>();
            }
            catch
            {
                return new List<Tree>();
            }
        }

        public async Task<bool> UpdateTree(Tree tree)
        {
            var response = await _http.PutAsJsonAsync($"Trees/{tree.Id}", tree);
            return response.IsSuccessStatusCode;
        }


    }
}
