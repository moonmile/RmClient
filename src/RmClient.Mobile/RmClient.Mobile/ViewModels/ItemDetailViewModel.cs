using System;
using Moonmile.Redmine.Model;
using RmClient.Mobile.Models;

namespace RmClient.Mobile.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel
    {
        public Issue Item { get; set; }
        public ItemDetailViewModel(Issue item = null)
        {
            Title = item?.Subject;
            Item = item;
        }
    }
}
