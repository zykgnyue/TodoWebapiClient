using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using TodoWebapiClient.model;

//Install-Package Microsoft.AspNet.WebApi.Client
//Install-Package System.Net.Http.Formatting.Extension
namespace TodoWebapiClient
{

    class Program
    {

        static HttpClient client = new HttpClient();
        static string todoApiuri = "api/todo";
        static void ShowTodoItem(TodoItem todoItem)
        {
            Console.WriteLine($"Name: {todoItem.Name}\tComplete: " +
                $"{todoItem.IsComplete}\tId: {todoItem.Id}");
        }

        static async Task<Uri> CreateTodoItemAsync(TodoItem todoItem)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(
                $"{todoApiuri}", todoItem);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }

        static async Task<TodoItem> GetTodoItemAsync(string path)
        {
            TodoItem todoItem = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                todoItem = await response.Content.ReadAsAsync<TodoItem>();
            }
            return todoItem;
        }

        static async Task<TodoItem> UpdateTodoItemAsync(TodoItem todoItem)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(
                $"{todoApiuri}/{todoItem.Id}", todoItem);
            response.EnsureSuccessStatusCode();

            // Deserialize the updated product from the response body.
            todoItem = await response.Content.ReadAsAsync<TodoItem>();
            return todoItem;
        }

        static async Task<HttpStatusCode> DeleteTodoItemAsync(long id)
        {
            HttpResponseMessage response = await client.DeleteAsync(
                $"{todoApiuri}/{id}");
            return response.StatusCode;
        }

        static void Main()
        {
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            // Update port # in the following line.
            client.BaseAddress = new Uri("http://localhost:5000/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                // Create a new product
                TodoItem todoItem = new TodoItem
                {
                    Id = 0,
                    Name = "Gizmo",
                    IsComplete = false,
                };

                var url = await CreateTodoItemAsync(todoItem);
                Console.WriteLine($"Created at {url}");

                // Get the product
                todoItem = await GetTodoItemAsync(url.PathAndQuery);
                ShowTodoItem(todoItem);

                // Update the product
                Console.WriteLine("Updating price...");
                todoItem.Name = "New " + todoItem.Name;
                await UpdateTodoItemAsync(todoItem);

                // Get the updated product
                todoItem = await GetTodoItemAsync(url.PathAndQuery);
                ShowTodoItem(todoItem);

                // Delete the product
                var statusCode = await DeleteTodoItemAsync(todoItem.Id);
                Console.WriteLine($"Deleted (HTTP Status = {(int)statusCode})");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }
    }
}
