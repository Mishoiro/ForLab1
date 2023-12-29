using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public enum FlightStatus {
    OnTime,
    Delayed,
    Cancelled,
    Boarding,
    InFlight
}
public class Flight {
    public string FlightNumber { get; set; }
    public string Airline { get; set; }
    public string Destination { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public string Gate { get; set; }
    public FlightStatus Status { get; set; }
    public TimeSpan Duration { get; set; }
    public string AircraftType { get; set; }
    public string Terminal { get; set; }
}
/*Робота з Json.file*/
public static class FilesEditor {
    
    public static void loadFlightsFromJson(string jsonFilePath, FlightInformationSystem flightSystem) {
        try {
            string jsonText = File.ReadAllText(jsonFilePath);
            var flightsData = JsonConvert.DeserializeObject<Dictionary<string, List<Flight>>>(jsonText);

            if (flightsData.ContainsKey("flights")) {
                foreach (var flight in flightsData["flights"]) {
                    flightSystem.addFlight(flight);
                }
            }
            else {
                Console.WriteLine("Invalid JSON format");
            }
        }
        catch (Exception ex) {
            Console.WriteLine($"Error loading flights from JSON: {ex.Message}");
        }
    }
    public static void saveToFile(List<Flight> flightSystem) {

        File.WriteAllText("D:/Visual files/LAB1/flights_temp_data.json", JsonConvert.SerializeObject(flightSystem, Formatting.Indented));
    }
}

/*Основний код*/
public class FlightInformationSystem {
    private List<Flight> flights = new List<Flight>();
    private List<Flight> filteredFlights = new List<Flight>();

    public List<Flight> Flights
    {
        get { return flights; }
        set { flights = value; }
    }

    public List<Flight> FilteredFlights
    {
        get { return filteredFlights; }
        set { filteredFlights = value; }
    }

    public void addFlight(Flight flight)
    {
        flights.Add(flight);
    }


    public List<Flight> GetFlightsByAirline(string airline) {
        foreach (var flight in flights) {
            if (flight.Airline.Equals(airline, StringComparison.OrdinalIgnoreCase))
            {
                filteredFlights.Add(flight);
            }
        }

        filteredFlights.Sort((flight1, flight2) => flight1.DepartureTime.CompareTo(flight2.DepartureTime));

        return filteredFlights;
    }

    public Flight GetFlight(string flightNumber) {
        return flights.Find(flight => flight.FlightNumber == flightNumber);
    }
    public void displayFlightInformation() {
        foreach (var flight in flights) {
            Console.WriteLine($"  Flight {flight.FlightNumber} - {flight.Airline}");
            Console.WriteLine($"  Destination: {flight.Destination}");
            Console.WriteLine($"  Departure Time: {flight.DepartureTime}");
            Console.WriteLine($"  Arrival Time: {flight.ArrivalTime}");
            Console.WriteLine($"  Gate: {flight.Gate}");
            Console.WriteLine($"  Status: {flight.Status}");
            Console.WriteLine($"  Duration: {flight.Duration}");
            Console.WriteLine($"  Aircraft Type: {flight.AircraftType}");
            Console.WriteLine($"  Terminal: {flight.Terminal}");
            Console.WriteLine();
        }
    }
    public List<Flight> GetDelayedFlightsSortedByDelayTime() {
        DateTime currentTime = DateTime.Now;

        foreach (var flight in flights) {
            if (flight.Status == FlightStatus.Delayed) {
                filteredFlights.Add(flight);
            }
        }
        filteredFlights.Sort((flight1, flight2) => (flight1.DepartureTime - currentTime).CompareTo(flight2.DepartureTime - currentTime));
        return filteredFlights;
    }

    public List<Flight> GetFlightsByDepartureDate(DateTime departureDate)
    {

        foreach (var flight in flights)
        {
            if (flight.DepartureTime.Date == departureDate.Date)
            {
                filteredFlights.Add(flight);
            }
        }

        filteredFlights.Sort((flight1, flight2) => flight1.DepartureTime.CompareTo(flight2.DepartureTime));

        return filteredFlights;
    }

    public List<Flight> GetFlightsByTimeAndDestination(DateTime startTime, DateTime endTime, string destination)
    {

        foreach (var flight in flights)
        {
            if (flight.DepartureTime >= startTime && flight.DepartureTime <= endTime &&
                flight.ArrivalTime >= startTime && flight.ArrivalTime <= endTime &&
                flight.Destination == destination)
            {
                filteredFlights.Add(flight);
            }
        }

        filteredFlights.Sort((flight1, flight2) => flight1.DepartureTime.CompareTo(flight2.DepartureTime));

        return filteredFlights;
    }

    public List<Flight> GetArrivedFlightsInTimeFrame(DateTime startTime, DateTime endTime) {
        foreach (var flight in flights) {
            if (flight.ArrivalTime >= startTime && flight.ArrivalTime <= endTime) {
                filteredFlights.Add(flight);
            }
        }

        filteredFlights.Sort((flight1, flight2) => flight1.ArrivalTime.CompareTo(flight2.ArrivalTime));

        return filteredFlights;
    }
}

/*1.Повернути всі рейси, які здійснюються певною
авіакомпанією. Рейси повинні бути відсортовані по часу
вильоту.*/
public class GetFlightsFromOneCompany {
    private FlightInformationSystem flightSystem;
    public GetFlightsFromOneCompany(FlightInformationSystem flightSystem)
    {
        this.flightSystem = flightSystem;
    }


    public void getFlightFromOneCompany(FlightInformationSystem flightSystem) {

        Console.WriteLine("Enter the airline name: ");
        string airlineName = Console.ReadLine();

        List<Flight> results = flightSystem.GetFlightsByAirline(airlineName);

        DisplayInfo.displayInfo(results);
    }

}

/*2. Повернути всі рейси, які на даний момент затримуються
(Status == FlightStatus.Delayed). Рейси повинні бути
відсортовані по часу затримки.*/
class GetDelayedFlights
{
    private FlightInformationSystem flightSystem;
    public GetDelayedFlights(FlightInformationSystem flightSystem)
    {
        this.flightSystem = flightSystem;
    }
    public void DisplayDelayedFlights()
    {
        List<Flight> results = flightSystem.GetDelayedFlightsSortedByDelayTime();
        DisplayInfo.displayInfo(results);
    }
}

/*3. Повернути всі рейси, які вилітають в певний день. Рейси
повинні бути відсортовані по часу вильоту.*/
class GetFlightsInOneDay {
    private FlightInformationSystem flightSystem;
    public GetFlightsInOneDay(FlightInformationSystem flightSystem) {
        this.flightSystem = flightSystem;
    }
    public void DisplayFlightsInOneDay() {
        Console.WriteLine("Enter the date (yyyy-MM-dd) to see flights departing on that day:");
        if (DateTime.TryParse(Console.ReadLine(), out DateTime selectedDate)) {
            List<Flight> results = flightSystem.GetFlightsByDepartureDate(selectedDate);
            DisplayInfo.displayInfo(results);
        }
        else {
            Console.WriteLine("Invalid date format.");
        }
    }
}

/*○ 4. Повернути всі рейси, які вилітають та прибувають у
вказаний проміжок часу (Наприклад: з 2023 - 05 - 1T00: 00:01 до
2023 - 05 - 31T23: 59:59) та мають вказаний пункт призначення.
Рейси повинні бути відсортовані по часу вильоту.*/
class GetFlightsInSpecifyTime
{
    private FlightInformationSystem flightSystem;
    public GetFlightsInSpecifyTime(FlightInformationSystem flightSystem)
    {
        this.flightSystem = flightSystem;
    }
    public void DisplayFlightsInSpecifyTime()
    {
        Console.WriteLine("Enter the start date and time (yyyy-MM-ddThh:mm:ss):");
        if (DateTime.TryParse(Console.ReadLine(), out DateTime startTime))
        {
            Console.WriteLine("Enter the end date and time (yyyy-MM-ddThh:mm:ss):");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime endTime))
            {
                Console.WriteLine("Enter the destination:");
                string destination = Console.ReadLine();

                List<Flight> results = flightSystem.GetFlightsByTimeAndDestination(startTime, endTime, destination);
                DisplayInfo.displayInfo(results);
            }
            else
            {
                Console.WriteLine("Invalid end date and time format.");
            }
        }
        else
        {
            Console.WriteLine("Invalid start date and time format.");
        }
    }
}

/*5. Повернути всі рейси, які прибули за останню годину або за
вказаний проміжок часу. Рейси повинні бути відсортовані по
часу прибуття.*/

class GetFlightsSomeTimeAgo
{
    private FlightInformationSystem flightSystem;
    public GetFlightsSomeTimeAgo(FlightInformationSystem flightSystem)
    {
        this.flightSystem = flightSystem;
    }
    public void DisplayFlightsSomeTimeAgo()
    {
        Console.WriteLine("Enter the time frame to check for flights (in hours):");
        if (double.TryParse(Console.ReadLine(), out double hoursAgo))
        {
            DateTime startTime = DateTime.Now.AddHours(-hoursAgo);
            DateTime endTime = DateTime.Now;

            List<Flight> results = flightSystem.GetArrivedFlightsInTimeFrame(startTime, endTime);
            DisplayInfo.displayInfo(results);
        }
        else
        {
            Console.WriteLine("Invalid input for time frame.");
        }
    }
}
/*Виводить данні на консось*/
public static class DisplayInfo {

    public static void displayInfo(List<Flight> results) {

        if (results.Count > 0) {
            Console.WriteLine("Searching Flights:");
            foreach (var flight in results) {
                Console.WriteLine();
                Console.WriteLine($"  Flight {flight.FlightNumber} - {flight.Airline}");
                Console.WriteLine($"  Destination: {flight.Destination}");
                Console.WriteLine($"  Departure Time: {flight.DepartureTime}");
                Console.WriteLine($"  Arrival Time: {flight.ArrivalTime}");
                Console.WriteLine($"  Gate: {flight.Gate}");
                Console.WriteLine($"  Status: {flight.Status}");
                Console.WriteLine($"  Duration: {flight.Duration}");
                Console.WriteLine($"  Aircraft Type: {flight.AircraftType}");
                Console.WriteLine($"  Terminal: {flight.Terminal}");
            }
        }
        else {
            Console.WriteLine("No flights found.");
        }
    }
}
/*Виводить меню*/
class Menu {
    public void displayMenu() {
        Console.WriteLine($"Menu:");
        Console.WriteLine($"1)  Show all flights;");
        Console.WriteLine($"2)  Show all flights that are made on a certain date by an airline; ");
        Console.WriteLine($"3)  Show all flights that are currently delayed; ");
        Console.WriteLine($"4)  Show all flights departing on a certain day; ");
        Console.WriteLine($"5)  Show all flights departing from and arriving at specified time period; ");
        Console.WriteLine($"6)  Show all flights that arrived in the last hour or specified time period; ");
        Console.WriteLine($"Choose your variant: ");
    }
    public void choice(FlightInformationSystem flightSystem) {
        string choice = Console.ReadLine();

        if (int.TryParse(choice, out int result)) {
            switch (result) {
                case 1:
                    flightSystem.displayFlightInformation();
                    break;
                case 2:
                    GetFlightsFromOneCompany flightsFromOneCompany = new GetFlightsFromOneCompany(flightSystem);
                    flightsFromOneCompany.getFlightFromOneCompany(flightSystem);
                    break;
                case 3:
                    GetDelayedFlights delayedFlights = new GetDelayedFlights(flightSystem);
                    delayedFlights.DisplayDelayedFlights();
                    break;
                case 4:
                    GetFlightsInOneDay flightsInOneDay = new GetFlightsInOneDay(flightSystem);
                    flightsInOneDay.DisplayFlightsInOneDay();
                    break;
                case 5:
                    GetFlightsInSpecifyTime flightsInSpecifyTime = new GetFlightsInSpecifyTime(flightSystem);
                    flightsInSpecifyTime.DisplayFlightsInSpecifyTime();
                    break;
                case 6:
                    GetFlightsSomeTimeAgo flightsSomeTimeAgoHandler = new GetFlightsSomeTimeAgo(flightSystem);
                    flightsSomeTimeAgoHandler.DisplayFlightsSomeTimeAgo();
                    break;
                default:
                    Console.WriteLine("Uknown command");
                    break;
            }
        }
        else {
            Console.WriteLine("Invalid format");
        }
    }
}
class Program {
    static void Main() {
        FlightInformationSystem flightSystem = new FlightInformationSystem();
        Menu menu = new Menu();
        FilesEditor.loadFlightsFromJson("D:/Visual files/LAB1/flights_data.json", flightSystem);

        menu.displayMenu();
        menu.choice(flightSystem);
        FilesEditor.saveToFile(flightSystem.FilteredFlights);
    }
}