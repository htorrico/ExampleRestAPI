using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ExampleRestAPI
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        HttpClient client;
        public  MainPage()
        {
            InitializeComponent();
            
            btnAdd.Clicked += async (sender, e) =>
            {
                await SaveTodoItemAsync(new TodoItem { id="5",name="hugo", notes="notas", done=true },true);
                listView.ItemsSource = await GetItems();
            };

        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            listView.ItemsSource = await GetItems();
        }
        public async Task<List<TodoItem>> GetItems()
        {
            List<TodoItem> Items = null;
            client = new HttpClient();

            //Uri uri = new Uri(string.Format(Constants.TodoItemsUrl, string.Empty));
            Uri uri = new Uri("http://192.168.1.3/todoapi/api/todoitems");
            HttpResponseMessage response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                Items = JsonConvert.DeserializeObject<List<TodoItem>>(content);
            }
            return Items;
        }

        public async Task SaveTodoItemAsync(TodoItem item, bool isNewItem = false)
        {
            //Uri uri = new Uri(string.Format(Constants.RestUrl, string.Empty));
            Uri uri = new Uri("http://192.168.1.3/todoapi/api/todoitems");
            try
            {
                string json = JsonConvert.SerializeObject(item);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;
                if (isNewItem)
                {
                    response = await client.PostAsync(uri, content);
                }
                else
                {
                    response = await client.PutAsync(uri, content);
                }

                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine(@"\tTodoItem successfully saved.");
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
        }




    }
    public class TodoItem
    {        
        public string id { get; set; }
        public string name { get; set; }
        public string notes { get; set; }
        public bool done { get; set; }

    }
}
