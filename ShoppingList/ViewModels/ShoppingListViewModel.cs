using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ShoppingList.ViewModels;
using ShoppingList.Services;
using System.Diagnostics;

namespace ShoppingList.ViewModels
{
    public class ShoppingListViewModel : INotifyPropertyChanged
    {
        private readonly MicroCmsService _service;

        public ObservableCollection<Item> Items { get; set; }
        public string NewItemName { get; set; }
        public bool IsNewItemUrgent { get; set; }
        public ICommand AddItemCommand { get; }
        public ICommand DeleteItemCommand { get; }
        public ICommand ClearAllItemsCommand { get; } // 全削除コマンド
        public ICommand ShareMessageCommand { get; }


        public ShoppingListViewModel()
        {
            _service = new MicroCmsService();
            Items = new ObservableCollection<Item>();
            AddItemCommand = new Command(AddItem);
            DeleteItemCommand = new Command<Item>(DeleteItem);
            RefreshItemsCommand = new Command(LoadItems);
            ClearAllItemsCommand = new Command(ClearAllItems);
            ShareMessageCommand = new Command(ShareMessage);
            LoadItems();
        }

        private async void LoadItems()
        {
            Items.Clear(); // リストを全削除
            //var items = await _service.GetItemsAsync();
            //if (items != null)
            //{
            //    foreach (var item in items)
            //    {
            //        Items.Add(item);
            //    }
            //}
            var items = await _service.GetItemAsync();
            if (items != null)
            {
                var sortedItems = items.OrderBy(item => item.CreatedAt).ToList(); // 追加された部分
                foreach (var item in sortedItems)
                {
                    Items.Add(item);
                }
            }
        }

        private async void AddItem()
        {
            if (string.IsNullOrWhiteSpace(NewItemName))
                return;

            var newItem = new Item
            {
                Name = NewItemName,
                IsUrgen = IsNewItemUrgent
            };

            await _service.AddItemAsync(newItem);

            NewItemName = string.Empty;
            IsNewItemUrgent = false;

            OnPropertyChanged(nameof(NewItemName));
            OnPropertyChanged(nameof(IsNewItemUrgent));

            LoadItems();
        }

        private async void DeleteItem(Item item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.Id))
                return;

            await _service.DeleteItemAsync(item.Id);
            Items.Clear();
            LoadItems();
        }

        private async void ClearAllItems()
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert("確認", "すべてのアイテムを削除しますか？", "Yes", "No");
            if (confirm)
            {
                // 全アイテムの削除を実行
                foreach (var item in Items.ToList()) // ToListを使用してアイテムのコピーを作成
                {
                    await _service.DeleteItemAsync(item.Id);
                }
                Items.Clear(); // ローカルのリストもクリア
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand RefreshItemsCommand { get; }

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private async void ShareMessage()
        {
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = "リストを更新しました",
                Title = "共有するメッセージ"
            });
        }
    }
}
