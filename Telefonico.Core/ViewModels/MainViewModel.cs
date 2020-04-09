namespace Telefonico.Core.ViewModels
{
    public class MainViewModel : ViewModelBase
    {

        private string _email;



        public string Email
        {
            set
            {
                _email = value;
            }
        }

        public MainViewModel()
        {
        }


    }

}
