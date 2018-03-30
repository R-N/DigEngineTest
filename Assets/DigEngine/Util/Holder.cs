using System;
namespace DigEngine
{
	public class Holder<T>
	{
		internal T value;
		public Holder(T value)
		{
			this.value = value;
		}
	}
}
