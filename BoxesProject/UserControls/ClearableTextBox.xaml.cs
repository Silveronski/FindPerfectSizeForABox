using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
namespace BoxesProject.UserControls
{   
    public partial class ClearableTextBox : UserControl , INotifyPropertyChanged
    {
        public ClearableTextBox()
        {
            DataContext = this;
            InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private string boundText;
        public string BoundText
        {
            get { return boundText; }
            set
            {
                boundText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BoundText"));
            }
        }

        private void txtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtInput.Text != string.Empty)
            {
                tbText.Visibility = Visibility.Hidden;
            }
            else tbText.Visibility = Visibility.Visible;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtInput.Text = string.Empty;
            txtInput.Focus();
        }        
    }
}
