namespace BoxesLibraryClass
{
    public class Box
    {
        public float Height { get; set; }
        public float Base { get; set; }
        public int Quantity { get; set; }
        public DateTime LastPurchasedAt { get; set; }
        public DateTime DateOfCreation { get; set; }       
        public string HashKey { get; private set; }
        public readonly int MaxQuantity;  
        
        public Box(float height, float Base, int quantity)
        {
            Height = height;
            this.Base = Base;   
            Quantity = quantity;            
            MaxQuantity = 1000;           
            LastPurchasedAt = default;
            DateOfCreation = DateTime.Now;           
            HashKey = GetHashKey(this);
        }

        public override string ToString()
        {
            return $"Height: {Height:F2} cm | Base: {Base:F2} cm² | Quantity: {Quantity} | Date of Creation: {DateOfCreation} | Last Purchased At: {LastPurchasedAt}";
        }

        private static string GetHashKey(Box box)
        {           
            return box.Height.ToString(); + "," + box.Base.ToString(); 
        }       

        public static void ValidateBoxProperties(string boxHeight, string boxBase, string boxQuantity)
        {
            bool isHeightValid = float.TryParse(boxHeight, out float heightF);
            bool isBaseValid = float.TryParse(boxBase, out float baseF);
            bool isQuantityValid = int.TryParse(boxQuantity, out int quantityI);

            if (isHeightValid == false) throw new InvalidCastException("Height Must be a floating point number!");
            if (isBaseValid == false) throw new InvalidCastException("Base Must be a floating point number!");
            if (isQuantityValid == false) throw new InvalidCastException("Quantity must be a whole number!");

            if (heightF < 1 || heightF > 500) throw new ArgumentException("Height Must be a positive number between 1-500");
            if (baseF < 1 || baseF > 500) throw new ArgumentException("Base Must be a positive number between 1-500");
            if (quantityI < 1) throw new ArgumentException("Quantity Must be a positive number");
        }
    }
}
