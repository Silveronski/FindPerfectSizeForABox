using BoxesLibraryClass;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
namespace BoxesProject.UserControls
{
    public partial class AddOrRestockBox : UserControl
    {
        Box box;
        Box boxBeforeRestock;
        Box retrievedBox;
        bool newBox;
        bool boxMaxQuantitySurpassed;
        public AddOrRestockBox()
        {
            InitializeComponent();
            btnSubmit.ButtonText = "Submit";
            btnReturn.ButtonText = "Return";
            btnSubmit.ButtonClickEvent += BtnSubmit_ButtonClickEvent;
            btnReturn.ButtonClickEvent += BtnReturn_ButtonClickEvent;
        }

        private void BtnSubmit_ButtonClickEvent(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!AreTextBoxesEmpty())
                {
                    Box.ValidateBoxProperties(heightX.txtInput.Text, basex.txtInput.Text, quantityX.txtInput.Text);
                    float Height = float.Parse(heightX.txtInput.Text);
                    float Base = float.Parse(basex.txtInput.Text);
                    int Quantity = int.Parse(quantityX.txtInput.Text);
                    box = new Box(Height, Base, Quantity);                   
                    if (DataBase.DoesBoxExist(box))
                    {
                        boxBeforeRestock = DataBase.FindBox(box.HashKey);
                        retrievedBox = DataBase.RestockBox(box);                         
                        if (DataBase.IsMaxQuantitySurpassed(retrievedBox)) // if box already exists, but the quantity that was entered surpassed its max quantity,
                                                                          // we update the box's quantity to its max quantity.
                        {
                            boxMaxQuantitySurpassed = true;
                            DataBase.SaveBoxInformation(retrievedBox);
                            boxMaxQuantitySurpassed = false;
                            MessageBox.Show($"This box already exists, but it surpassed its maximum quantity. \nThe quantity has been decreased back" +
                                        $" to its maximum of {retrievedBox.MaxQuantity}", "Quantity Updated", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else // if box exists and quantity does not surpass max quantity, we update the box's quantity to the quantity that was entered.
                        {
                            DataBase.SaveBoxInformation(retrievedBox);
                            MessageBox.Show($"This box already exists, its quantity has been updated to {retrievedBox.Quantity}", "Quantity Updated", MessageBoxButton.OK, MessageBoxImage.Information);
                        }                                      
                    }
                    else // if box does not exist
                    {
                        newBox = true;
                        DataBase.AddBox(box);
                        if (DataBase.IsMaxQuantitySurpassed(box)) // if new box has surpassed its maximum quantity, we decrease it back to itx max quantity.
                        {
                            DataBase.SaveBoxInformation(box);
                            MessageBox.Show($"The box has surpassed its maximum quantity. \nThe quantity has been decreased back to its maximum of {box.MaxQuantity}. " +
                                            $"\nThe box been successfully created with these parameters: \nHeight: {box.Height}  Base: {box.Base}" +
                                            $"  Quantity: {box.Quantity}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            DataBase.SaveBoxInformation(box);
                            MessageBox.Show($"The box has been successfully created with these parameters: \nHeight: {box.Height}  Base: {box.Base}" +
                                            $"  Quantity: {box.Quantity}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);                            
                        }                       
                        newBox = false;                                             
                    }
                    ClearAllTextBoxes();
                    ReturnToMainMenu();
                }               
            }
            catch (InvalidCastException ex)
            {
                MessageBox.Show(ex.Message,"ERROR",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }            
            catch (DirectoryNotFoundException ex)
            {
                if (newBox)
                {
                    DataBase.RemoveBox(box);
                    newBox = false;
                }
                if (boxMaxQuantitySurpassed)
                {
                    retrievedBox.Quantity = boxBeforeRestock.Quantity;
                    boxMaxQuantitySurpassed = false;
                }
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception)
            {
                MessageBox.Show("An unexpected error has occurred!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnReturn_ButtonClickEvent(object sender, RoutedEventArgs e)
        {
            ReturnToMainMenu();
            ClearAllTextBoxes();
        }

        private void ReturnToMainMenu()
        {
            Window mainWindow = Window.GetWindow(this);
            Grid mainMenu = (Grid)mainWindow.FindName("mainMenu");
            mainMenu.Visibility = Visibility.Visible;
            Grid addOrRestockBox = (Grid)mainWindow.FindName("AddOrRestockBoxGrid");
            addOrRestockBox.Visibility = Visibility.Collapsed;
        }

        private void ClearAllTextBoxes()
        {
            heightX.txtInput.Text = string.Empty;
            basex.txtInput.Text = string.Empty;
            quantityX.txtInput.Text = string.Empty;
        }

        private bool AreTextBoxesEmpty()
        {
            if (heightX.txtInput.Text == string.Empty || basex.txtInput.Text == string.Empty || quantityX.txtInput.Text == string.Empty)
            {
                MessageBox.Show("You must fill all the text boxes!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return true;
            }
            return false;
        }
    }
}