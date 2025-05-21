using Linux_Mint.MVVM.Model;
using Linux_Mint.MVVM.ViewModel;

namespace Linux_Mint.MVVM.View.MainPage
{
    public partial class EditPostPopup : ContentPage
    {
        public EditPostPopup(UserPost post)
        {
            InitializeComponent();
            BindingContext = new EditPostViewModel(post);
        }
    }
}