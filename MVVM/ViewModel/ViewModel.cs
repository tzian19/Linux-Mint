using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Maui.ApplicationModel;
using System.Windows.Input;

using Linux_Mint.MVVM.Model;

namespace Linux_Mint.MVVM.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public UserProfile LoggedInUser { get; protected set; }
        private protected readonly HttpClient client;
        private protected readonly JsonSerializerOptions _serializerOptions;
        private protected readonly string baseUrl = "https://680f29be67c5abddd1940e6d.mockapi.io";

        public ViewModelBase()
        {
            OnSwiped = new Command<object>(async (param) => await SwipeTask(param));
            client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions { WriteIndented = true };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // ✅ Add this method for property binding support
        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        // Swipe gesture on flyout pages
        public ICommand OnSwiped { get; }
        private async Task SwipeTask(object param)
        {
            if (param is string dir && dir == "Right" && Application.Current.MainPage is FlyoutPage flyoutPage)
            {
                flyoutPage.IsPresented = true;
            }
        }
    }
}
