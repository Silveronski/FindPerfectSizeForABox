using BoxesLibraryClass;
using BoxesProject.UserControls;
using System;
using System.IO;
using System.Windows;
namespace BoxesProject
{   
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                AssignButtonNames();
                AssignButtonEvents();
                DataBase.LoadBoxes();               
            }
            catch (DirectoryNotFoundException ex) // will pop up an error message if loading the boxes into the system failed.
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
            catch (Exception)
            {
                MessageBox.Show("An unexpected error has occurred!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        private void BtnExit_ButtonClickEvent(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnRemoveBoxes_ButtonClickEvent(object sender, RoutedEventArgs e)
        {
            if (!AreThereAnyBoxes()) return;

            mainMenu.Visibility = Visibility.Collapsed;
            RemoveBoxesGrid.Visibility = Visibility.Visible;            
        }

        private void BtnShowBoxes_ButtonClickEvent(object sender, RoutedEventArgs e)
        {
            if (!AreThereAnyBoxes()) return;   
            
            mainMenu.Visibility = Visibility.Collapsed;
            ShowBoxesGrid.Visibility = Visibility.Visible;
            ShowBoxes showBoxes = (ShowBoxes)ShowBoxesGrid.Children[0];
            showBoxes.DisplayBoxes();                     
        }

        private void BtnAddRestockBox_ButtonClickEvent(object sender, RoutedEventArgs e)
        {
            mainMenu.Visibility = Visibility.Collapsed;
            AddOrRestockBoxGrid.Visibility = Visibility.Visible;
        } 
        
        private bool AreThereAnyBoxes()
        {
            int boxesCount = DataBase.GetBoxesCount();
            if (boxesCount == 0)
            {
                MessageBox.Show("There are currently no boxes available for display!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }       

        private void AssignButtonNames()
        {
            btnAddRestockBox.ButtonText = "Add/Restock Box";
            btnShowBoxes.ButtonText = "Display Available Boxes";
            btnRemoveBoxes.ButtonText = "Display Removable Boxes";
            btnExit.ButtonText = "Exit";           
        }

        private void AssignButtonEvents()
        {
            btnAddRestockBox.ButtonClickEvent += BtnAddRestockBox_ButtonClickEvent;
            btnShowBoxes.ButtonClickEvent += BtnShowBoxes_ButtonClickEvent;
            btnRemoveBoxes.ButtonClickEvent += BtnRemoveBoxes_ButtonClickEvent;
            btnExit.ButtonClickEvent += BtnExit_ButtonClickEvent;
        }
    }
}