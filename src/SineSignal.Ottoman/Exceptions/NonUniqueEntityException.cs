using System;

namespace SineSignal.Ottoman.Exceptions
{
	public class NonUniqueEntityException : Exception
	{
		public NonUniqueEntityException()
		{
		}
		
		public NonUniqueEntityException(string message) : base(message)
		{
		}
		
		public NonUniqueEntityException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
