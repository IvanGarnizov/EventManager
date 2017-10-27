namespace EventManager.Client.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Data;

    using Models;

    public class EventManager
    {
        private const string DateTimeFormat = "yyyy/MM/ddTHH:mm:ss";
        private const string DateTimePresentationFormat = "dd/MM/yyyy HH:mm:ss";

        private EventManagerContext context;

        public EventManager()
        {
            this.context = new EventManagerContext();
        }

        public void ReadEvents()
        {
            var events = this.context.Events;

            if (events.Count() > 0)
            {
                foreach (var eventEntity in events)
                {
                    Console.WriteLine($"~~~~~~~~~~~\nId: {eventEntity.Id}\nName: {eventEntity.Name}\nLocation: {eventEntity.Location}\nFrom: {eventEntity.StartDateAndTime.ToString(DateTimePresentationFormat)} To: {eventEntity.EndDateAndTime.ToString(DateTimePresentationFormat)}\n~~~~~~~~~~~");
                }
            }
            else
            {
                Console.WriteLine("There are no events.");
            }
        }

        public void AddEvent(List<string> arguments)
        {
            string name = arguments[0];
            string location = arguments[1];

            if (IsCorrectDateTimeFormat(arguments[2], out DateTime startDateAndTime))
            {
                if (IsCorrectDateTimeFormat(arguments[3], out DateTime endDateAndTime))
                {
                    if (CheckDatesAreInCorrectSequence(startDateAndTime, endDateAndTime))
                    {
                        var eventEntity = new Event()
                        {
                            Name = name,
                            Location = location,
                            StartDateAndTime = startDateAndTime,
                            EndDateAndTime = endDateAndTime
                        };

                        this.context.Events.Add(eventEntity);
                        this.context.SaveChanges();

                        Console.WriteLine($"Successfully created event {name}.");
                    }
                }
            }
        }

        public void UpdateEvent(List<string> arguments)
        {
            if (IsInteger(arguments[0], out int id))
            {
                arguments.RemoveAt(0);
                
                if (EventExists(id, out Event eventEntity))
                {
                    string name = String.Empty;
                    string location = String.Empty;
                    DateTime startDateAndTime = DateTime.Now;
                    DateTime endDateAndTime = DateTime.Now;
                    bool startDateMatches = true;
                    bool endDateMatches = true;
                    bool startDateChanged = false;
                    bool endDateChanged = false;
                    bool hasIncorrectField = false;

                    foreach (var pair in arguments)
                    {
                        if (pair.Contains("="))
                        {
                            string[] parts = pair.Split('=');
                            string field = parts[0];
                            string value = parts[1];

                            switch (field)
                            {
                                case "name":
                                    name = value;

                                    break;
                                case "location":
                                    location = value;

                                    break;
                                case "startDateAndTime":
                                    if (!IsCorrectDateTimeFormat(value, out startDateAndTime))
                                    {
                                        startDateMatches = false;
                                    }
                                    else
                                    {
                                        startDateChanged = true;
                                    }

                                    break;
                                case "endDateAndTime":
                                    if (!IsCorrectDateTimeFormat(value, out endDateAndTime))
                                    {
                                        endDateMatches = false;
                                    }
                                    else
                                    {
                                        endDateChanged = true;
                                    }

                                    break;
                                default:
                                    hasIncorrectField = true;

                                    Console.WriteLine($"Field {field} does not exist in an event.");

                                    break;
                            }
                        }
                        else
                        {
                            hasIncorrectField = true;

                            Console.WriteLine("Incorrect data. Field name must be followed by '='.");
                        }
                    }

                    if (startDateMatches && endDateMatches && !hasIncorrectField)
                    {
                        if (name != String.Empty)
                        {
                            eventEntity.Name = name;
                        }

                        if (location != String.Empty)
                        {
                            eventEntity.Location = location;
                        }

                        bool isCorrectDateSequence = true;

                        if (startDateChanged && endDateChanged)
                        {
                            if (CheckDatesAreInCorrectSequence(startDateAndTime, endDateAndTime))
                            {
                                eventEntity.StartDateAndTime = startDateAndTime;
                                eventEntity.EndDateAndTime = endDateAndTime;
                            }
                            else
                            {
                                isCorrectDateSequence = false;
                            }
                        }
                        else if (startDateChanged)
                        {
                            if (CheckDatesAreInCorrectSequence(startDateAndTime, eventEntity.EndDateAndTime))
                            {
                                eventEntity.StartDateAndTime = startDateAndTime;
                            }
                            else
                            {
                                isCorrectDateSequence = false;
                            }
                        }
                        else if (endDateChanged)
                        {
                            if (CheckDatesAreInCorrectSequence(eventEntity.StartDateAndTime, endDateAndTime))
                            {
                                eventEntity.EndDateAndTime = endDateAndTime;
                            }
                            else
                            {
                                isCorrectDateSequence = false;
                            }
                        }

                        if (isCorrectDateSequence)
                        {
                            this.context.SaveChanges();

                            Console.WriteLine($"Successfully updated event {eventEntity.Name}.");
                        }
                    }
                }
            }
        }

        public void DeleteEvent(List<string> arguments)
        {
            if (IsInteger(arguments[0], out int id))
            {
                if (EventExists(id, out Event eventEntity))
                {
                    this.context.Events.Remove(eventEntity);
                    this.context.SaveChanges();

                    Console.WriteLine($"Successfully deleted event {eventEntity.Name}.");
                }
            }
        }

        private bool IsInteger(string value, out int number)
        {
            if (!int.TryParse(value, out number))
            {
                Console.WriteLine("Id is not an integer.");

                return false;
            }

            return true;
        }

        private bool IsCorrectDateTimeFormat(string value, out DateTime dateTime)
        {
            if (!DateTime.TryParseExact(value, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
            {
                Console.WriteLine($"All dates must be in format {DateTimeFormat}.");

                return false;
            }

            return true;
        }

        private bool EventExists(int id, out Event eventEntity)
        {
            eventEntity = this.context.Events
                    .FirstOrDefault(e => e.Id == id);

            if (eventEntity == null)
            {
                Console.WriteLine($"Event with id {id} doesn't exist.");

                return false;
            }

            return true;
        }

        private bool CheckDatesAreInCorrectSequence(DateTime firstDate, DateTime secondDate)
        {
            if (secondDate < firstDate)
            {
                Console.WriteLine("An event can't end before it has started.");

                return false;
            }

            return true;
        }
    }
}
