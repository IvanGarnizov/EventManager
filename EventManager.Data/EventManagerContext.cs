namespace EventManager.Data
{
    using System.Data.Entity;

    using Models;

    public class EventManagerContext : DbContext
    {
        public EventManagerContext()
            : base("name=EventManagerContext")
        {
        }

        public virtual DbSet<Event> Events { get; set; }
    }
}