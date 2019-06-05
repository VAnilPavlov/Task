namespace TaskMeneger
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class TaskContext : DbContext
    {        
        public TaskContext()
            : base("name=Model1")
        {
        }

        public virtual DbSet<Task> TaskDb { set; get; }
    }    
}