using System.Globalization;
using System.Net.Http.Json;

namespace ConsoleClient;

class Program
{
    private static readonly HttpClient client = new()
    {
        BaseAddress = new Uri("http://localhost:5227") // API base URL without /api suffix
    };

    static async Task Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("\n===== Employee Management System =====");
            Console.WriteLine("1. Add Record");
            Console.WriteLine("2. Edit Record");
            Console.WriteLine("3. Delete Record");
            Console.WriteLine("4. List All Records");
            Console.WriteLine("5. Exit");
            Console.Write("Choose an option: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await AddRecord();
                    break;
                case "2":
                    await EditRecord();
                    break;
                case "3":
                    await DeleteRecord();
                    break;
                case "4":
                    await ListRecords();
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }
        }
    }

    private static async Task AddRecord()
    {
        var employee = new Employee();

        Console.Write("Enter Full Name: ");
        var fullName = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(fullName))
        {
            Console.WriteLine("Full Name is required.");
            return;
        }

        Console.Write("Enter Age (18–65): ");
        if (!int.TryParse(Console.ReadLine(), out int age) || age < 18 || age > 65)
        {
            Console.WriteLine("Invalid Age.");
            return;
        }

        Console.Write("Enter Date of Joining (dd/MM/yyyy): ");
        if (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime doj))
        {
            Console.WriteLine("Invalid Date format.");
            return;
        }

        employee.FullName = fullName;
        employee.Age = age;
        employee.DateOfJoining = doj;

        var response = await client.PostAsJsonAsync("api/employees", employee);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Employee added successfully.");
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error: {error}");
        }
    }

    private static async Task EditRecord()
    {
        Console.Write("Enter Employee ID to edit: ");
        if (!Guid.TryParse(Console.ReadLine(), out Guid id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        var existing = await client.GetFromJsonAsync<Employee>($"api/employees/{id}");
        if (existing == null)
        {
            Console.WriteLine("Employee not found.");
            return;
        }

        Console.Write($"Enter Full Name ({existing.FullName}): ");
        var fullName = Console.ReadLine();
        fullName = string.IsNullOrWhiteSpace(fullName) ? existing.FullName : fullName;

        Console.Write($"Enter Age ({existing.Age}): ");
        var ageInput = Console.ReadLine();
        int age = string.IsNullOrWhiteSpace(ageInput) ? existing.Age : int.Parse(ageInput);

        Console.Write($"Enter Date of Joining (dd/MM/yyyy) ({existing.DateOfJoining:dd/MM/yyyy}): ");
        var dojInput = Console.ReadLine();
        DateTime doj = string.IsNullOrWhiteSpace(dojInput)
            ? existing.DateOfJoining
            : DateTime.ParseExact(dojInput, "dd/MM/yyyy", CultureInfo.InvariantCulture);

        existing.FullName = fullName;
        existing.Age = age;
        existing.DateOfJoining = doj;

        var response = await client.PutAsJsonAsync($"api/employees/{id}", existing);
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Employee updated successfully.");
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error: {error}");
        }
    }

    private static async Task DeleteRecord()
    {
        Console.Write("Enter Employee ID to delete: ");
        if (!Guid.TryParse(Console.ReadLine(), out Guid id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        var response = await client.DeleteAsync($"api/employees/{id}");
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Employee deleted.");
        }
        else
        {
            Console.WriteLine("Employee not found.");
        }
    }

    private static async Task ListRecords()
    {
        var employees = await client.GetFromJsonAsync<List<Employee>>("api/employees");
        if (employees == null || employees.Count == 0)
        {
            Console.WriteLine("No records found.");
            return;
        }

        Console.WriteLine("\n--- Employee List ---");
        foreach (var emp in employees)
        {
            Console.WriteLine($"ID: {emp.Id}");
            Console.WriteLine($"Name: {emp.FullName}");
            Console.WriteLine($"Age: {emp.Age}");
            Console.WriteLine($"Date of Joining: {emp.DateOfJoining:dd/MM/yyyy}");
            Console.WriteLine("-----------------------------");
        }
    }

    public class Employee
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int Age { get; set; }
        public DateTime DateOfJoining { get; set; }
    }
}
