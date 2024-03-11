using BoxesLibraryClass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
namespace BoxesProject.UserControls
{
    public partial class RemoveBoxes : UserControl
    {
        List<Box> boxesToRemove;
        int amountOfHours;
        public RemoveBoxes()
        {
            InitializeComponent();
            AssignButtonNames();
            AssignButtonEvents();
        }

        private void BtnReturn_ButtonClickEvent(object sender, RoutedEventArgs e)
        {
            ReturnToMainMenu();            
        }

        private void BtnRemove_ButtonClickEvent(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                try
                {
                    PopulateList();
                    MessageBoxResult result = MessageBox.Show($"Are you sure you want to remove the box with the following parameters:" +
                                $" \nHeight: {boxesToRemove[listBox.SelectedIndex].Height}  Base: {boxesToRemove[listBox.SelectedIndex].Base}",
                                "Confirmation",MessageBoxButton.YesNo,MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        DataBase.RemoveBox(boxesToRemove[listBox.SelectedIndex]);
                        DataBase.DeleteBoxFile(boxesToRemove[listBox.SelectedIndex]);
                        boxesToRemove.Remove(boxesToRemove[listBox.SelectedIndex]);
                        listBox.ItemsSource = null;
                        listBox.ItemsSource = boxesToRemove;
                        MessageBox.Show("The box has been successfully removed!","Box Removed", MessageBoxButton.OK, MessageBoxImage.Information);                        
                    }                   
                }
                catch (DirectoryNotFoundException ex) 
                {
                    DataBase.AddBox(boxesToRemove[listBox.SelectedIndex]);
                    MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (BoxNotFoundException ex)
                {                   
                    MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a box to remove!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRemoveAll_ButtonClickEvent(object sender, RoutedEventArgs e)
        {          
            try
            {
                PopulateList();                
            }
            catch (BoxNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }           
            MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete all {boxesToRemove.Count} boxes?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                foreach (var box in boxesToRemove)
                {
                    try
                    {
                        DataBase.RemoveBox(box);
                        DataBase.DeleteBoxFile(box);
                    }
                    catch (DirectoryNotFoundException ex)
                    {
                        DataBase.AddBox(box);
                        MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);                       
                        return;
                    }
                }
                boxesToRemove = null;
                RevertTextToDefault();
                MessageBox.Show("All the boxes have been successfully removed!", "Boxes Removed", MessageBoxButton.OK, MessageBoxImage.Information);
            }            
        }

        private void BtnFind_ButtonClickEvent(object sender, RoutedEventArgs e)
        {
            if (timeText.txtInput.Text != string.Empty)
            {
                try
                {
                    amountOfHours = int.Parse(timeText.txtInput.Text);
                    listBox.ItemsSource = DataBase.GetRemovableBoxes(amountOfHours);
                    title.Text = $"box/boxes that haven't been purchased more than {amountOfHours} hours";
                }
                catch (InvalidCastException)
                {
                    MessageBox.Show("Please enter a whole number!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (BoxNotFoundException ex) // if no box was found.
                {
                    RevertTextToDefault();
                    MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);                   
                }
                catch (FormatException ex)
                {
                    RevertTextToDefault();
                    MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please enter amount of hours!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void PopulateList()
        {
            boxesToRemove = null;
            boxesToRemove = (List<Box>)listBox.ItemsSource;
            if (boxesToRemove == null) throw new BoxNotFoundException("Please find a box before trying to remove!");
        }

        private void ReturnToMainMenu()
        {
            Window mainWindow = Window.GetWindow(this);
            Grid mainMenu = (Grid)mainWindow.FindName("mainMenu");
            mainMenu.Visibility = Visibility.Visible;
            Grid RemoveBoxesGrid = (Grid)mainWindow.FindName("RemoveBoxesGrid");
            RemoveBoxesGrid.Visibility = Visibility.Collapsed;
            RevertTextToDefault();
        } 
        
        private void RevertTextToDefault()
        {
            title.Text = "Find boxes that haven't been purchased T hours:";
            listBox.ItemsSource = null;
            timeText.txtInput.Text = string.Empty;
        }

        private void AssignButtonNames()
        {
            btnRemoveAll.ButtonText = "Remove all Boxes";
            btnRemove.ButtonText = "Remove Specific Box";
            btnReturn.ButtonText = "Return";
            btnFind.ButtonText = "Search";
        }

        private void AssignButtonEvents()
        {           
            btnRemoveAll.ButtonClickEvent += BtnRemoveAll_ButtonClickEvent;
            btnRemove.ButtonClickEvent += BtnRemove_ButtonClickEvent;
            btnReturn.ButtonClickEvent += BtnReturn_ButtonClickEvent;
            btnFind.ButtonClickEvent += BtnFind_ButtonClickEvent;
        }       
    }
}