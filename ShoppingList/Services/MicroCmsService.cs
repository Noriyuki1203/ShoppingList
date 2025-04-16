using Newtonsoft.Json;
using ShoppingList.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingList.Services
{
    public class MicroCmsService
    {
        public readonly HttpClient _httpClient;

        public MicroCmsService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://shopping-list.microcms.io/api/v1/")
            };
            _httpClient.DefaultRequestHeaders.Add("X-MICROCMS-API-KEY", "kT4DYRTJKyl6cjanFgZ5d58h3aCjy6WCY5SQ");
        }

        public async Task<List<Item>> GetItemAsync()
        {
            List<Item> allItems = new List<Item>();
            int offset = 0;
            int limit = 100; // microCMSの最大取得件数
            int totalCount = 0;

            try
            {
                do
                {
                    var response = await _httpClient.GetStringAsync($"shoppinglist?limit={limit}&offset={offset}");
                    var jsonResponse = JsonConvert.DeserializeObject<MicroCmsResponse>(response);

                    if (jsonResponse?.Contents != null)
                    {
                        allItems.AddRange(jsonResponse.Contents);
                        totalCount = jsonResponse.Contents.Count;
                    }
                    offset += limit;
                }
                while (offset < totalCount);
                //return allItems;
                return allItems.OrderByDescending(item => item.CreatedAt).ToList();

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return new List<Item>(); ;
            }
        }

        public async Task AddItemAsync(Item item)
        {
            var jsonContent = JsonConvert.SerializeObject(new
            {
                Name = item.Name,
                IsUrgen = item.IsUrgen // フィールド名を "IsUrgen" に修正
            });
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            try
            {
                Console.WriteLine("Sending POST request with JSON content:");
                Console.WriteLine(jsonContent);

                var response = await _httpClient.PostAsync("shoppinglist", content);
                Console.WriteLine("Response status code: " + response.StatusCode);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Response error content: " + errorContent);
                }

                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
        }

        public async Task DeleteItemAsync(string id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"shoppinglist/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
        }
    }

    public class MicroCmsResponse
    {
        public List<Item> Contents { get; set; }
        public int TotalCount { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }
    }
}
