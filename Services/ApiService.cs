using System.Net.Http.Json;
using GreenGuard.Models;

namespace GreenGuard.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;

        // ===== PLATFORM-SAFE BASE URL =====
#if ANDROID
        private const string BaseUrl = "https://10.0.2.2:7113/api/";
#elif IOS
        private const string BaseUrl = "https://localhost:7113/api/";
#elif MACCATALYST
        private const string BaseUrl = "https://localhost:7113/api/";
#else
        // Windows only
        private const string BaseUrl = "https://localhost:7113/api/";
#endif

        public ApiService()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    (message, cert, chain, errors) => true
            };

            Console.WriteLine(">>> USING BASE URL: " + BaseUrl);

            _http = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl),
                Timeout = TimeSpan.FromSeconds(20)
            };
        }

        // ================= INTERNAL USERS ================= 

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

        // ================= EXTERNAL USERS ================= 

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

        // ================= ANALYTICS ================= 

        public async Task<AdminDashboardResponse?> GetAdminDashboard()
        {
            return await _http.GetFromJsonAsync<AdminDashboardResponse>("Analytics/dashboard");
        }

        // ================= TREES ================= 

        public async Task<bool> AddTree(Tree tree)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("Trees", tree);

                Console.WriteLine("=== ADD TREE RESPONSE ===");
                Console.WriteLine("Status: " + response.StatusCode);
                Console.WriteLine("Body: " + await response.Content.ReadAsStringAsync());
                Console.WriteLine("=========================");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine("=== ADD TREE EXCEPTION ===");
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<List<Tree>> GetTrees()
        {
            try
            {
                Console.WriteLine(">>> CALLING GET /Trees");

                var response = await _http.GetAsync("Trees");
                var body = await response.Content.ReadAsStringAsync();

                Console.WriteLine("Status: " + response.StatusCode);
                Console.WriteLine("Body: " + body);

                if (!response.IsSuccessStatusCode)
                    return new List<Tree>();

                var trees = await response.Content.ReadFromJsonAsync<List<Tree>>();
                Console.WriteLine(">>> RESULT COUNT: " + (trees?.Count ?? 0));

                return trees ?? new List<Tree>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("=== GET TREES EXCEPTION ===");
                Console.WriteLine(ex);
                return new List<Tree>();
            }
        }

        public async Task<bool> DeleteTree(string id)
        {
            try
            {
                var response = await _http.DeleteAsync($"Trees/{id}");
                Console.WriteLine("Status: " + response.StatusCode);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine("DELETE TREE ERROR:");
                Console.WriteLine(ex);
                return false;
            }
        } 

        public async Task<List<InternalUser>> GetLeaders()
        {
            return await _http.GetFromJsonAsync<List<InternalUser>>("InternalUsers/leaders")
                   ?? new List<InternalUser>();
        }

        public async Task<bool> AddLeader(InternalUser leader)
        {
            var res = await _http.PostAsJsonAsync("InternalUsers", leader);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateLeader(InternalUser leader)
        {
            var res = await _http.PutAsJsonAsync($"InternalUsers/{leader.Id}", leader);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteLeader(string id)
        {
            var res = await _http.DeleteAsync($"InternalUsers/{id}");
            return res.IsSuccessStatusCode;
        }
        public async Task<List<InternalUser>> GetVolunteers()
        {
            return await _http.GetFromJsonAsync<List<InternalUser>>("InternalUsers/volunteers")
                   ?? new List<InternalUser>();
        }

        public async Task<bool> AddVolunteer(InternalUser volunteer)
        {
            var res = await _http.PostAsJsonAsync("InternalUsers", volunteer);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateVolunteer(InternalUser volunteer)
        {
            var res = await _http.PutAsJsonAsync($"InternalUsers/volunteers/{volunteer.Id}", volunteer);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteVolunteer(string id)
        {
            var res = await _http.DeleteAsync($"InternalUsers/volunteers/{id}");
            return res.IsSuccessStatusCode;
        }
        // ---------- ZONES ----------
        public async Task<List<Zone>> GetZones()
        {
            return await _http.GetFromJsonAsync<List<Zone>>("Zones") ?? new();
        }

        public async Task<bool> AddZone(Zone zone)
        {
            var response = await _http.PostAsJsonAsync("Zones", zone);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateZone(Zone zone)
        {
            var response = await _http.PutAsJsonAsync($"Zones/{zone.Id}", zone);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteZone(string id)
        {
            var response = await _http.DeleteAsync($"Zones/{id}");
            return response.IsSuccessStatusCode;
        } 
        public async Task<List<VolunteerUpdate>> GetPendingUpdates()
        {
            return await _http.GetFromJsonAsync<List<VolunteerUpdate>>("VolunteerUpdates")
                   ?? new List<VolunteerUpdate>();
        }

        public async Task<bool> ApproveUpdate(string id)
        {
            var response = await _http.PutAsync($"VolunteerUpdates/approve/{id}", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RejectUpdate(string id)
        {
            var response = await _http.PutAsync($"VolunteerUpdates/reject/{id}", null);
            return response.IsSuccessStatusCode;
        }
        public async Task<bool> SubmitPlantationUpdate(PlantationUpdate update)
        {
            var res = await _http.PostAsJsonAsync("PlantationUpdates", update);
            return res.IsSuccessStatusCode;
        }
        public async Task<List<PlantationUpdate>> GetAllUpdates()
        {
            return await _http.GetFromJsonAsync<List<PlantationUpdate>>("PlantationUpdates")
                   ?? new List<PlantationUpdate>();
        }
        public async Task<bool> PurchaseTree(string treeId, string userId, int qty)
        {
            var req = new
            {
                TreeId = treeId,
                UserId = userId,
                Quantity = qty
            };

            var res = await _http.PostAsJsonAsync("Trees/purchase", req);

            return res.IsSuccessStatusCode;
        }


    }
}
