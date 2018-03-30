using System.Collections;
using System.Collections.Generic;

namespace DigEngine{
	public struct ColliderPair {

		public Collider self;
		public Collider other;

		public ColliderPair(Collider self, Collider other){
			this.self = self;
			this.other = other;
		}

		public override int GetHashCode(){
			return self.GetHashCode() ^ other.GetHashCode();
		}

		public override bool Equals(object obj){
			if (obj is ColliderPair) {
				ColliderPair other = (ColliderPair)obj;
				return this.self == other.self && this.other == other.other;
			}
			return false;
		}
	}
}
