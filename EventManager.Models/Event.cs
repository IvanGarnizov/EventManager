namespace EventManager.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Event
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Location { get; set; }

        public DateTime StartDateAndTime { get; set; }

        public DateTime EndDateAndTime { get; set; }
    }
}
