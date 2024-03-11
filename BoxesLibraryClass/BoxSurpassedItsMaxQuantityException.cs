namespace BoxesLibraryClass
{      
    public class BoxNotFoundException : Exception
    {
        public BoxNotFoundException() { }
        public BoxNotFoundException(string message) : base(message) { }
    }
}
