using System;
using System.Collections.Generic;

namespace StoreManager
{
    class Program
    {
        static List<Product> productList = new List<Product>();

        static void Main(string[] args)
        {
            GenerateMenu();
        }

        public static void GenerateMenu()
        {
            while (true)
            {
                Console.WriteLine("PRODUCT MANAGEMENT PROGRAM");
                Console.WriteLine("-----------------------------");
                Console.WriteLine("1. Add product records");
                Console.WriteLine("2. Display product records");
                Console.WriteLine("3. Delete product by ID");
                Console.WriteLine("4. Exit");
                Console.WriteLine("Please enter your choice:");
                // choose the option
                var choice = 0;
                while (true)
                {
                    try
                    {
                        var strChoice = Console.ReadLine();
                        choice = Int32.Parse(strChoice);
                        break;
                    }
                    catch (FormatException e)
                    {
                        Console.WriteLine("Please enter a number.");
                    }
                }

                switch (choice)
                {
                    case 1:
                        Add();
                        break;
                    case 2:
                        Display();
                        break;
                    case 3:
                        Delete();
                        break;
                    case 4:
                        Console.WriteLine("Bye. See you again!");
                        Environment.Exit(1);
                        break;
                    default:
                        Console.WriteLine("This is not a valid choice, please try again");
                        break;
                }
            }
        }

        public static void Add()
        {
            Console.WriteLine("Enter the product's ID:");
            string id = Console.ReadLine();
            Console.WriteLine("Enter the product's name:");
            string name = Console.ReadLine();
            Console.WriteLine("Enter the product's price:");
            decimal price;
            while (true)
            {
                try
                {
                    var strChoice = Console.ReadLine();
                    price = Decimal.Parse(strChoice);
                    if (price < 0)
                    {
                        throw new FormatException();
                    }
                    else
                    {
                        break;
                    }
                }
                catch (FormatException e)
                {
                    Console.WriteLine("Please enter an unsign number.");
                }
            }

            Console.WriteLine("Are you sure want to add a product with below information?");
            Console.WriteLine("ID: {0} | Name: {1} | Price: {2}", id, name, "$" + price);
            Console.WriteLine("Press 'Y' to confirm the action. Press other buttons to cancel the action");
            if (Console.ReadKey().Key == ConsoleKey.Y)
            {
                Console.WriteLine("Your product has been added to the list");
                var newProduct = new Product(id, name, price);
                productList.Add(newProduct);
            }
            else
            {
                Console.WriteLine("Your product has NOT been added to the list");
            }
            Console.ReadLine();
        }

        public static void Display()
        {
            Console.WriteLine("You have {0} product in your list.", productList.Count);
            Console.WriteLine(String.Format("{0, 15} | {1, 20} | {2, 15}", "Product ID", "Product Name", "Price"));
            Console.WriteLine(String.Format("{0, 15} | {1, 20} | {2, 15}", "---------------", "--------------------", "---------------"));
            foreach (var p in productList)
            {
                Console.WriteLine(String.Format("{0, 15} | {1, 20} | {2, 15}", p.Id, p.Name, "$" + p.Price));
            }
            Console.ReadLine();
        }

        public static void Delete()
        {
            // WARNING: This code assume that product ID is UNIQUE (2 product do not have the same ID)
            
            Console.WriteLine("Enter the product's ID you want to delete:");
            string id = Console.ReadLine();
            int found = -1;
            for (int i = 0; i < productList.Count; i++)
            {
                if (productList[i].Id == id)
                {
                    found = i;
                }
            }

            if (found != -1)
            {
                Console.WriteLine("Found a product with the ID and below information:");
                Console.WriteLine("ID: {0} | Name: {1} | Price: {2}", productList[found].Id, productList[found].Name, "$" + productList[found].Price);
                Console.WriteLine("Press 'Y' to delete the product. Press other buttons to cancel the action");
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    Console.WriteLine("Your product has been deleted from the list");
                    productList.RemoveAt(found);
                }
                else
                {
                    Console.WriteLine("Your product has NOT been deleted");
                }
                Console.ReadLine();
            }
            else if (found == -1)
            {
                Console.WriteLine("Can not find any product with id: {0}", id);
                Console.ReadLine();
            }
        }
    }
}