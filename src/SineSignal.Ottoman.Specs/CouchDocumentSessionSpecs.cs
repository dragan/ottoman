using System;

using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using SineSignal.Ottoman.Specs.Framework;

namespace SineSignal.Ottoman.Specs
{
	public class CouchDocumentSessionSpecs
	{
	}
	
	public class Employee
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Login { get; set; }
		
		public override bool Equals (object obj)
		{
			if (obj == null)
				return false;
			
			Employee employee = obj as Employee;
			if (employee == null)
				return false;
			
			return (this.Id == employee.Id) && (this.Name == employee.Name) && (this.Login == employee.Login);
		}
		
		public override int GetHashCode ()
		{
			return Id.GetHashCode() ^ Name.GetHashCode() ^ Login.GetHashCode();
		}
	}
}
