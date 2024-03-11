using System.Text.Json;
namespace BoxesLibraryClass
{
    public static class DataBase
    {
        static Dictionary<string, Box> boxes = new Dictionary<string, Box>();
        static string dataBase = @"C:\Users\Ron\Desktop\projects\Projects\BoxesProjectWPF\DataBase";

        public static string GetDataBaseName()
        {
            return dataBase;
        }
        public static bool IsMaxQuantitySurpassed(Box box)
        {
            if (box.Quantity > box.MaxQuantity)
            {
                box.Quantity = box.MaxQuantity;
                return true;
            }
            return false;
        }      

        public static bool DoesBoxExist(Box box)
        {
            if (boxes.ContainsKey(box.HashKey)) return true;
            return false;
        }

        public static Box FindBox(string boxHashKey)
        {
            Box box = boxes[boxHashKey];
            return box;
        }

        public static Box RestockBox(Box box)
        {
            Box retrievedBox = boxes[box.HashKey];
            retrievedBox.Quantity += box.Quantity;
            return retrievedBox;
        }

        public static void AddBox(Box box)
        {
            if (!boxes.ContainsKey(box.HashKey))
            {
                boxes.Add(box.HashKey, box);
            }               
        }

        public static void RemoveBox(Box box)
        {
            if (boxes.ContainsKey(box.HashKey))
            {
                boxes.Remove(box.HashKey);
            }          
        }

        public static ICollection<Box> GetBoxes()
        {
            return boxes.Values.Where(box => box.Quantity > 0).ToList();
        }  
        
        public static int GetBoxesCount()
        {
            return boxes.Count;
        }      

        public static List<Box> GetRemovableBoxes(int hours)
        {
            List<Box> removableBoxes = new List<Box>();
            foreach (var box in boxes.Values)
            {
                if (IsRemovable(box, hours)) removableBoxes.Add(box);
            }
            if (removableBoxes.Count == 0) throw new BoxNotFoundException($"There isn't any box that hasn't been purchased more than {hours} hours!");
            return removableBoxes;
        }
        private static bool IsRemovable(Box box,int hours)
        {
            TimeSpan timeDifference = DateTime.Now - box.DateOfCreation;
            return timeDifference.TotalHours > hours;
        }

        public static Box[] FindBoxForGift(float heightReceived, float baseReceived, int quantityReceived)
        {
            Box[] matchedBoxes = new Box[boxes.Count];
            Box[] oneBoxArr = new Box[1];
            int index = 0;
            string hashKey = heightReceived.ToString() + "," + baseReceived.ToString();
            if (boxes.ContainsKey(hashKey)) 
            {
                Box retrievedBox = boxes[hashKey];
                oneBoxArr[0] = retrievedBox;
                if (oneBoxArr[0].Quantity >= quantityReceived) return oneBoxArr; //base case, if the height and base that was received
                                                                                 //matches perfectly to a box, with enough quantity, we return it.           
            }   
            
            const float MAXSTRAY = 1.2f; // max stray we allow is 20% upwards from original base and height.
            foreach (var box in boxes)
            {
                if (box.Value.Base <= baseReceived * MAXSTRAY && box.Value.Base >= baseReceived // box.Value.Base should be between baseReceived and baseReceived + 20%
                    && box.Value.Height <= heightReceived * MAXSTRAY && box.Value.Height >= heightReceived) // box.Value.Height should be between heightReceived and heightReceived + 20%
                {
                    matchedBoxes[index] = box.Value;
                    index++;
                }               
            }          
            Array.Resize(ref matchedBoxes, index);
            if (matchedBoxes.Length == 0) throw new BoxNotFoundException("There are no boxes that match your description!");

            Array.Sort(matchedBoxes, new BoxComparer()); // we sort the array in order to get the smallest boxes first.
            int tempQuantity = 0;
            index = 0;
            while (quantityReceived > tempQuantity && index < matchedBoxes.Length)  
            {
                tempQuantity += matchedBoxes[index].Quantity;
                index++;               
            }
            Array.Resize(ref matchedBoxes, index);
            return matchedBoxes;
        }        

        public static void SaveBoxInformation(Box box)
        {            
            if (!Directory.Exists(dataBase)) throw new DirectoryNotFoundException("Data base directory could not be found!");                                  
            string directory = dataBase + "/" + box.HashKey;
            string fileName = "BoxInfo.json";
            Directory.CreateDirectory(directory);
            string filePath = Path.Combine(directory, fileName);
            string boxSerialized = JsonSerializer.Serialize(box);
            File.WriteAllText(filePath, boxSerialized);                      
        }

        public static void LoadBoxes()
        {          
            if (!Directory.Exists(dataBase)) throw new DirectoryNotFoundException("Data base directory could not be found!");                      
            foreach (string filePath in Directory.GetFiles(dataBase, "BoxInfo.json", SearchOption.AllDirectories))
            {
                string boxDeserialized = File.ReadAllText(filePath);
                Box? box = JsonSerializer.Deserialize<Box>(boxDeserialized);
                AddBox(box);
            }                                  
        }

        public static void DeleteBoxFile(Box box)
        {
            if (!Directory.Exists(dataBase)) throw new DirectoryNotFoundException("Data base directory could not be found!");
            string directory = dataBase + "/" + box.HashKey;         
            if (Directory.Exists(directory)) Directory.Delete(directory, true);
            else throw new DirectoryNotFoundException("The box directory you wish to delete could not be found!");
        }
    }
}