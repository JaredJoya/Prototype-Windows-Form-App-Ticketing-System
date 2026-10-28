using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System.Buffers.Text;

namespace tempProj
{
    public class ApiService
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static string baseUrl = "http://192.168.2.55:5075";
        public static string BaseUrl => baseUrl;
        public static void SetBaseUrl(string url)
        {
            baseUrl = url.TrimEnd('/');
        }

        public async Task<bool> IsAdminAsync()
        {
            var response = await httpClient.GetAsync($"{BaseUrl}/permissions");
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<PermissionsResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return result?.IsAdmin ?? false;
        }

        public async Task<List<Ticket>> GetTicketsAsync()
        {
            var response = await httpClient.GetAsync($"{BaseUrl}/tickets");
            response.EnsureSuccessStatusCode();
            string json = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<List<Ticket>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            }) ?? new List<Ticket>();
        }

        public async Task<Ticket> GetTicketAsync(string id)
        {
            string url = BaseUrl + "/tickets/" + Uri.EscapeDataString(id);
            var response = await httpClient.GetAsync(url);
            if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();
            string json = await response.Content.ReadAsStringAsync();

            Ticket ticket = JsonConvert.DeserializeObject<Ticket>(json);

            if (ticket == null)
            {
                throw new Exception("Unable to read ticket data.");
            }

            return ticket;
        }

        public async Task CreateTicketAsync(Ticket ticket)
        {
            var payload = new
            {
                ticket.ComputerName,
                ticket.CreatedBy,
                ticket.Description,
                ticket.Status,
                ticket.Title
            };

            string json = JsonConvert.SerializeObject(payload);
            using(var content = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                var response = await httpClient.PostAsync(BaseUrl + "/tickets", content);
                response.EnsureSuccessStatusCode();
            }
        }

        public async Task UpdateTicketAsync(string id, Ticket ticket)
        {
            var payload = new
            {
                ticket.Title,
                ticket.Description,
                ticket.Status,

                CreatedBy = Environment.UserName,
                ComputerName = Environment.MachineName
            };

            string json = JsonConvert.SerializeObject(payload);
            using(var content = new StringContent(json,Encoding.UTF8, "application/json"))
            {
                var response = await httpClient.PutAsync(BaseUrl + "/tickets/" + Uri.EscapeDataString(id), content);
                response.EnsureSuccessStatusCode();
            }
        }

        public async Task DeleteTicketAsync(string id, string createdBy, string computerName)
        {
            string url = BaseUrl +
                $"/tickets/{Uri.EscapeDataString(id)}" +
                $"?createdBy={Uri.EscapeDataString(createdBy)}" +
                $"&computerName={Uri.EscapeDataString(computerName)}";

            var response = await httpClient.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
        }
    }

    public class Ticket
    {
        public string Id { get; set; } = "";
        public string ComputerName { get; set; } = "";
        public string CreatedAt { get; set; } = "";
        public string CreatedBy { get; set; } = "";
        public string Description { get; set; } = "";
        public string Status { get; set; } = "";
        public string Title { get; set; } = "";
        public string UpdatedAt { get; set; } = "";

    }

    public class PermissionsResponse
    {
        public bool IsAdmin {  get; set; }
    }
}
