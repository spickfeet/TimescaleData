namespace WebAPI.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public string EntityType { get; }
        public Guid Id { get; }

        public EntityNotFoundException(string entityType, Guid id)
            : base($"Entity '{entityType}' with ID {id} not found.")
        {
            EntityType = entityType;
            Id = id;
        }
    }
}
