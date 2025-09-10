using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        IToDoService service = new ToDoService();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("==== Todo App ====");
            Console.WriteLine("1. Add item");
            Console.WriteLine("2. List items");
            Console.WriteLine("3. Mark item complete");
            Console.WriteLine("4. Delete item");
            Console.WriteLine("5. Exit");
            Console.Write("Choose an option: ");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    Console.Write("Enter title: ");
                    var title = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(title))
                    {
                        service.addItem(title.Trim());
                        Console.WriteLine("Added.");
                    }
                    else
                    {
                        Console.WriteLine("Title cannot be empty.");
                    }
                    Pause();
                    break;

                case "2":
                    Console.WriteLine(service.listItems());
                    Pause();
                    break;

                case "3":
                    Console.WriteLine(service.listItems());
                    Console.Write("Enter ID to mark complete: ");
                    if (int.TryParse(Console.ReadLine(), out int idToComplete))
                    {
                        var ok = service.markItemComplete(idToComplete);
                        Console.WriteLine(ok ? "Marked complete." : "Item not found.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid ID.");
                    }
                    Pause();
                    break;

                case "4":
                    Console.WriteLine(service.listItems());
                    Console.Write("Enter ID to delete: ");
                    if (int.TryParse(Console.ReadLine(), out int idToDelete))
                    {
                        service.deleteItem(idToDelete);
                        Console.WriteLine("Deleted if existed.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid ID.");
                    }
                    Pause();
                    break;

                case "5":
                    service.exit();
                    return;

                default:
                    Console.WriteLine("Invalid option.");
                    Pause();
                    break;
            }
        }
    }

    

    static void Pause()
    {
        Console.WriteLine();
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
    }
}
