using BoxesLibraryClass;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
namespace BoxesProject.UserControls
{
    public partial class ShowBoxes : UserControl
    {
        Box[] boxesForGift;
        int quantityForGift;
        float baseForGift;
        float heightForGift;
        bool perfectMatch;
        bool perfectMatchNotEnoughQuantity;
        bool severalOrOneMatches;
        bool severalOrOneMatchesNotEnoughQuantity;
        DateTime boxLastPurchasedAt;              
        public ShowBoxes()
        {
            InitializeComponent();
            AssignButtonNames();
            AssignButtonEvents();
        }           

        private void BtnShowAllBoxes_ButtonClickEvent(object sender, RoutedEventArgs e)
        {
            DisplayBoxes();
            ClearAllTextBoxes();
            SetBoolPropertiesToFalse();
        }

        private void BtnBuy_ButtonClickEvent(object sender, RoutedEventArgs e) // buy boxes
        {
            string dataBaseFolder = DataBase.GetDataBaseName();
            if (!Directory.Exists(dataBaseFolder))
            {
                MessageBox.Show("Data base directory could not be found!","ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (perfectMatch) // we buy one box with enough quantity.
            {
                BoxPerfectMatch();
                ClearAllTextBoxes();
                ReturnToMainMenu();
            }
            else if (perfectMatchNotEnoughQuantity) // we buy one box without enough quantity, so we remove it from the system.
            {
                BoxPerfectMatchNotEnoughQuantity();
                ClearAllTextBoxes();
                ReturnToMainMenu();                              
            }
            else if (severalOrOneMatches) // we buy one or several boxes with enough quantity.
            {
                BoxSeveralOrOneMatches();
                ClearAllTextBoxes();
                ReturnToMainMenu();
            }
            else if (severalOrOneMatchesNotEnoughQuantity) // we buy one or several boxes without enough quantity, so we remove it from the system.
            {
                BoxSeveralOrOneMatchesNotEnoughQuantity();
                ClearAllTextBoxes();
                ReturnToMainMenu();
            }
            else
            {
                MessageBox.Show("You need to first find a box before buying one!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnFind_ButtonClickEvent(object sender, RoutedEventArgs e) // find boxes
        {
            if (baseText.txtInput.Text != string.Empty && heightText.txtInput.Text != string.Empty && quantityText.txtInput.Text != string.Empty)
            {              
                try
                {
                    Box.ValidateBoxProperties(heightText.txtInput.Text, baseText.txtInput.Text, quantityText.txtInput.Text); // check if each property is valid (not string and etc).
                    ParseBoxProperties(); 
                    boxesForGift = DataBase.FindBoxForGift(heightForGift, baseForGift, quantityForGift); 
                    if (boxesForGift.Length == 1 && boxesForGift[0].Base == baseForGift && boxesForGift[0].Height == heightForGift)
                    {
                        if (boxesForGift[0].Quantity < quantityForGift) // perfect match but but not enough quantity.
                        {
                            MessageBox.Show("The system found a perfect match but not enough quantity", "Box Found", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            perfectMatchNotEnoughQuantity = true;
                        }
                        else // perfect match with enough quantity.
                        {
                            MessageBox.Show("The system found a perfect match!", "Box Found", MessageBoxButton.OK, MessageBoxImage.Information);
                            perfectMatch = true;
                        }

                        listBox.ItemsSource = null;
                        listBox.ItemsSource = boxesForGift; // displays the box that was found.
                    }
                    else
                    {
                        int tempQuantity = 0;
                        foreach (var box in boxesForGift) // we add all of the boxes quantity to see if its enough for the gift.
                        {
                            tempQuantity += box.Quantity;
                        }                        
                        if (tempQuantity < quantityForGift) // several matches or one without enough quantity.
                        {
                            MessageBox.Show("The system found several boxes or one that fit your description, \nWith no more than 20% stray but not enough quantity!", "Boxes Found", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                            severalOrOneMatchesNotEnoughQuantity = true;
                            listBox.ItemsSource = null;
                            listBox.ItemsSource = boxesForGift; // displays the boxes that were found.
                        }
                        else // several matches or one with enough quantity.
                        {
                            MessageBox.Show("The system found several boxes or one that fit your description with no more than 20% stray!", "Boxes Found", MessageBoxButton.OK, MessageBoxImage.Information);
                            severalOrOneMatches = true;  
                            listBox.ItemsSource = null;
                            listBox.ItemsSource = boxesForGift; // displays the boxes that were found.
                        }
                    }                                    
                }
                catch (BoxNotFoundException ex) // if no box was found, an error message will pop up.
                {                   
                    MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    SetBoolPropertiesToFalse();
                    DisplayBoxes();
                }
                catch (InvalidCastException ex)
                {                   
                    MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (ArgumentException ex)
                {                    
                    MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception)
                {
                    MessageBox.Show("An unexpected error has occurred!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please enter the base, height and quantity of the gift!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BoxPerfectMatch()
        {
            perfectMatch = false;
            boxesForGift[0].Quantity -= quantityForGift; // subtract the wanted quantity from the box.
            if (boxesForGift[0].Quantity == 0) // if there is no more quantity left we remove it.
            {
                try
                {
                    DataBase.RemoveBox(boxesForGift[0]);
                    DataBase.DeleteBoxFile(boxesForGift[0]);
                    MessageBox.Show($"{quantityForGift} units of this Box has been successfully sold! \nIts quantity has been decreased to 0 and" +
                                    $" it will be erased from the system.", "Box Sold", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (DirectoryNotFoundException ex)
                {
                    boxesForGift[0].Quantity += quantityForGift;
                    DataBase.AddBox(boxesForGift[0]);
                    MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else // there is enough quantity after we take the wanted quantity.
            {
                try
                {                    
                    boxLastPurchasedAt = boxesForGift[0].LastPurchasedAt;
                    // we save this parameter in case we get an exception and need to revert it.

                    boxesForGift[0].LastPurchasedAt = DateTime.Now;                  
                    DataBase.SaveBoxInformation(boxesForGift[0]);
                    MessageBox.Show($"{quantityForGift} units of this Box has been successfully sold! \nIts quantity has been decreased to " +
                                    $"{boxesForGift[0].Quantity}", "Box Sold", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (DirectoryNotFoundException ex)
                {
                    boxesForGift[0].Quantity += quantityForGift;
                    boxesForGift[0].LastPurchasedAt = boxLastPurchasedAt;                   
                    MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BoxPerfectMatchNotEnoughQuantity()
        {
            try
            {
                perfectMatchNotEnoughQuantity = false;
                DataBase.RemoveBox(boxesForGift[0]);
                DataBase.DeleteBoxFile(boxesForGift[0]);
                MessageBox.Show($"{boxesForGift[0].Quantity} units of this Box has been successfully sold! \nIts quantity has been decreased" +
                                 $" to 0 and it will be erased from the system.", "Box Sold", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (DirectoryNotFoundException ex)
            {
                DataBase.AddBox(boxesForGift[0]);
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BoxSeveralOrOneMatches()
        {
            severalOrOneMatches = false;
            if (boxesForGift.Length == 1) // if we found only 1 box which is not a perfect match.
            {
                boxesForGift[0].Quantity -= quantityForGift;
                if (boxesForGift[0].Quantity == 0) // if the box's quantity is 0 after the subtraction, we remove it.
                {
                    try
                    {
                        DataBase.RemoveBox(boxesForGift[0]);
                        DataBase.DeleteBoxFile(boxesForGift[0]);
                        MessageBox.Show($"{quantityForGift} units of this Box has been successfully sold! \nIts quantity has been decreased to 0 and it will" +
                                    " be erased from the system.", "Box Sold ", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (DirectoryNotFoundException ex)
                    {
                        boxesForGift[0].Quantity += quantityForGift;
                        DataBase.AddBox(boxesForGift[0]);
                        MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else // the box's quantity is not 0 after the subtraction, we update it.
                {
                    try
                    {
                        boxLastPurchasedAt = boxesForGift[0].LastPurchasedAt;

                        // we save this parameter in case we get an exception and need to revert it.

                        boxesForGift[0].LastPurchasedAt = DateTime.Now;
                        DataBase.SaveBoxInformation(boxesForGift[0]);
                        MessageBox.Show($"{quantityForGift} units of this Box has been successfully sold! \nIts quantity has been decreased to {boxesForGift[0].Quantity}",
                                        "Box Sold", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (DirectoryNotFoundException ex)
                    {
                        boxesForGift[0].Quantity += quantityForGift;
                        boxesForGift[0].LastPurchasedAt = boxLastPurchasedAt;
                        MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }               
            }
            else // if there is more than 1 box.
            {
                int tempQuantity = quantityForGift;
                for (int i = 0; i < boxesForGift.Length - 1; i++) // we remove all the boxes from the system except the last one.
                {
                    try
                    {
                        tempQuantity -= boxesForGift[i].Quantity; // we remove each and every box's quantity from tempQuantity so we know how much to subtract from the last box.
                        DataBase.RemoveBox(boxesForGift[i]);
                        DataBase.DeleteBoxFile(boxesForGift[i]);
                    }
                    catch (DirectoryNotFoundException ex)
                    {
                        DataBase.AddBox(boxesForGift[i]);
                        MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                try
                {
                    boxLastPurchasedAt = boxesForGift[boxesForGift.Length - 1].LastPurchasedAt;
                    // we save this parameter in case we get an exception and need to revert it.

                    boxesForGift[boxesForGift.Length - 1].Quantity -= tempQuantity; // we subtract the quantity that was left from the last box.
                    boxesForGift[boxesForGift.Length - 1].LastPurchasedAt = DateTime.Now;

                    DataBase.SaveBoxInformation(boxesForGift[boxesForGift.Length - 1]);
                    MessageBox.Show($"{quantityForGift} units of {boxesForGift.Length} different Boxes have been successfully sold! \nTheir quantity has been decreased to 0 and they will be erased from the system. \nExcept one, which has been decreased to" +
                            $" {boxesForGift[boxesForGift.Length - 1].Quantity}", $"{boxesForGift.Length} Boxes Sold", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (DirectoryNotFoundException ex)
                {
                    boxesForGift[boxesForGift.Length - 1].Quantity += tempQuantity;
                    boxesForGift[boxesForGift.Length - 1].LastPurchasedAt = boxLastPurchasedAt;                   
                    MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BoxSeveralOrOneMatchesNotEnoughQuantity()
        {
            severalOrOneMatchesNotEnoughQuantity = false;
            int tempQuantity = 0;
            foreach (var box in boxesForGift) // because several boxes were found without enough quantity, we remove all of them from the system.
            {
                try
                {
                    tempQuantity += box.Quantity;
                    DataBase.RemoveBox(box);
                    DataBase.DeleteBoxFile(box);
                }
                catch (DirectoryNotFoundException ex)
                {
                    DataBase.AddBox(box);
                    MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            if (boxesForGift.Length == 1)
            {
                MessageBox.Show($"{boxesForGift[0].Quantity} units of this Box has been successfully sold! \nIts quantity has been decreased to 0 and will" +
                                " be erased from the system.", "Box Sold", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"{tempQuantity} units of {boxesForGift.Length} different Boxes have been successfully sold! \nTheir quantity has been decreased to 0 and they will" +
                                $" be erased from the system.", $"{boxesForGift.Length} Boxes Sold", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnReturn_ButtonClickEvent(object sender, RoutedEventArgs e)
        {
            ClearAllTextBoxes();
            ReturnToMainMenu();
        }
        private void ReturnToMainMenu()
        {
            Window mainWindow = Window.GetWindow(this);
            Grid mainMenu = (Grid)mainWindow.FindName("mainMenu");
            mainMenu.Visibility = Visibility.Visible;
            Grid ShowBoxesGrid = (Grid)mainWindow.FindName("ShowBoxesGrid");
            ShowBoxesGrid.Visibility = Visibility.Collapsed;
            SetBoolPropertiesToFalse();
        }

        public void DisplayBoxes()
        {            
            listBox.ItemsSource = null;
            listBox.ItemsSource = DataBase.GetBoxes();
        }
        
        private void ParseBoxProperties()
        {
            quantityForGift = int.Parse(quantityText.txtInput.Text);
            baseForGift = float.Parse(baseText.txtInput.Text);
            heightForGift = float.Parse(heightText.txtInput.Text);
        }
        
        private void ClearAllTextBoxes()
        {
            heightText.txtInput.Text = string.Empty;
            baseText.txtInput.Text = string.Empty;
            quantityText.txtInput.Text = string.Empty;
        }

        private void SetBoolPropertiesToFalse()
        {
            perfectMatch = false;
            perfectMatchNotEnoughQuantity = false;
            severalOrOneMatches = false;
            severalOrOneMatchesNotEnoughQuantity = false;
        }

        private void AssignButtonNames()
        {
            btnReturn.ButtonText = "Return";
            btnFind.ButtonText = "Search";
            btnBuy.ButtonText = "Buy";
            btnShowAllBoxes.ButtonText = "Display all Boxes";            
        }

        private void AssignButtonEvents()
        {
            btnReturn.ButtonClickEvent += BtnReturn_ButtonClickEvent;
            btnFind.ButtonClickEvent += BtnFind_ButtonClickEvent;
            btnBuy.ButtonClickEvent += BtnBuy_ButtonClickEvent;
            btnShowAllBoxes.ButtonClickEvent += BtnShowAllBoxes_ButtonClickEvent;
        }
    }
}