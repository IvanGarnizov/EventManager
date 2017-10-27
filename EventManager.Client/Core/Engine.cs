namespace EventManager.Client.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Engine
    {
        private const string TerminatingString = "stop";

        private string input;
        private EventManager eventManager;

        public Engine()
        {
            this.eventManager = new EventManager();
        }

        public void Run()
        {
            this.input = Console.ReadLine();

            while (input != TerminatingString)
            {
                List<string> arguments = input.Split(' ').ToList();
                string command = arguments[0];

                arguments.RemoveAt(0);

                switch (command)
                {
                    case "Read":
                        this.eventManager.ReadEvents();

                        break;
                    case "Create":
                        if (arguments.Count == 4)
                        {
                            this.eventManager.AddEvent(arguments);
                        }
                        else
                        {
                            Console.WriteLine("When creating a new event, the program expects 4 arguments - name, location, start date and time and end date and time.");
                        }

                        break;
                    case "Update":
                        int argumentsCount = arguments.Count;

                        if (1 < argumentsCount && argumentsCount <= 5)
                        {
                            this.eventManager.UpdateEvent(arguments);
                        }
                        else
                        {
                            Console.WriteLine("When updating an event, the program expects from 2 to 5 arguments - id and the fields you wish to update in format {field}=value, where {field} is one of the following: [name, location, startDateAndTime, endDateAndTime].");
                        }

                        break;
                    case "Delete":
                        if (arguments.Count == 1)
                        {
                            this.eventManager.DeleteEvent(arguments);
                        }
                        else
                        {
                            Console.WriteLine("When deleting an event, the program expects 1 argument - id.");
                        }

                        break;
                    default:
                        Console.WriteLine($"Command {command} is not supported");

                        break;
                }

                input = Console.ReadLine();
            }
        }
    }
}
