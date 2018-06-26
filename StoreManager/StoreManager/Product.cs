namespace StoreManager
{
    public class Product
    {
        private string id;
        private string name;
        private decimal price;

        public Product()
        {
        }

        public Product(string id, string name, decimal price)
        {
            this.id = id;
            this.name = name;
            this.price = price;
        }

        public string Id
        {
            get => id;
            set => id = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public decimal Price
        {
            get => price;
            set => price = value;
        }
    }
}