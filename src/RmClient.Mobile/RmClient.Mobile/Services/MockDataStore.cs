using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RmClient.Mobile.Models;
using Moonmile.Redmine.Model;
using Moonmile.Redmine;

namespace RmClient.Mobile.Services
{
    public class MockDataStore : IDataStore<Issue>
    {
        List<Issue> items;

        public MockDataStore()
        {
#if false
            items = new List<Issue>();
            var mockItems = new List<Issue>
            {
                new Issue { Id = 1, Subject = "First item", Description="This is an item description." },
                new Issue { Id = 2, Subject = "Second item", Description="This is an item description." },
                new Issue { Id = 3, Subject = "Third item", Description="This is an item description." },
                new Issue { Id = 4, Subject = "Fourth item", Description="This is an item description." },
                new Issue { Id = 5, Subject = "Fifth item", Description="This is an item description." },
                new Issue { Id = 6, Subject = "Sixth item", Description="This is an item description." }
            };

            foreach (var item in mockItems)
            {
                items.Add(item);
            }
#else
            init();
#endif
        }


        public async void init()
        {
            var sv = new RedmineService();
            sv.ApiKey = "7ee40c509bc59555251e2fd757f2e4a6e816989f";
            sv.BaseUrl = "http://openccpm.com/redmine/";
            int pid = 10;
            this.items = await sv.GetIssues(pid);
        }


        public async Task<bool> AddItemAsync(Issue item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Issue item)
        {
            var oldItem = items.Where((Issue arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var oldItem = items.Where((Issue arg) => arg.Id == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<Issue> GetItemAsync(int id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Issue>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
}