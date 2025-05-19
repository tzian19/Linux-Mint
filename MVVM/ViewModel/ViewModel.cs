using Linux_Mint.MVVM.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;

namespace Linux_Mint.MVVM.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        protected readonly HttpClient client;
        protected readonly JsonSerializerOptions _serializerOptions;
        protected readonly string baseUrl = "https://680f29be67c5abddd1940e6d.mockapi.io";

        private UserProfile _loggedInUser;

        public UserProfile LoggedInUser
        {
            get => _loggedInUser;
            set => SetProperty(ref _loggedInUser, value);
        }

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

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

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