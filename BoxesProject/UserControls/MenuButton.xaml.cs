using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
namespace BoxesProject.UserControls
{  
    public partial class MenuButton : UserControl , INotifyPropertyChanged
    {
        public MenuButton()
        {
            DataContext = this;
            InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private string buttonText;
        public string ButtonText
        {
            get { return buttonText; }
            set
            {
                buttonText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ButtonText"));
            }
        }

        public event RoutedEventHandler ButtonClickEvent;       
        private void btnMain_Click(object sender, RoutedEventArgs e)
        {
            ButtonClickEvent?.Invoke(this, e);
        }
    }
}
