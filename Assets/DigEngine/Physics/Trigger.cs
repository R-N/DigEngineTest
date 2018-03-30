using System;
namespace DigEngine
{
	public struct Trigger : ICloneable
	{
		private Collider _self;
		public Collider self
		{
			get
			{
				return _self;
			}
		}
		private Collider _other;
		public Collider other
		{
			get
			{
				return _other;
			}
		}
		public Trigger(Collider self, Collider other)
		{
			_self = self;
			_other = other;
		}
		
		public object Clone()
		{
			return GetClone();
		}

		public Trigger GetClone()
		{

			return new Trigger(
				(Collider)self.GetClone(),
				(Collider)other.GetClone()
				);
		}
		public Trigger GetClone(long timeStamp)
		{

			return new Trigger(
				(Collider)self.GetClone(timeStamp),
				(Collider)other.GetClone(timeStamp)
				);
		}
	}
}
