using System;

namespace RallyDakar.CustomExceptions
{
	public class EntityNotFoundException : Exception
	{
        public EntityNotFoundException()
        {
        }
        public EntityNotFoundException(string message)
            : base(message)
        {
        }
    }
}
