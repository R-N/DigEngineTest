using System;

namespace DigEngine
{
	public abstract class Behaviour : Component
	{
		public Behaviour (GameObject gameObject) : base(gameObject){
		}



		public virtual void PrePhysicsUpdate1(float dt){
			
		}

		public virtual void PrePhysicsUpdate2(float dt){
			
		}
		

		public virtual void PostPhysicsUpdate1(float dt){
			
		}

		public virtual void PostPhysicsUpdate2(float dt){
			
		}

		public virtual void OnCollisionEnter(Collision col)
		{
			
		}

		public virtual void OnCollisionStay(Collision col)
		{

		}

		public virtual void OnCollisionExit(Collision col)
		{

		}

		public virtual void OnTriggerEnter(Trigger trig)
		{

		}

		public virtual void OnTriggerStay(Trigger trig)
		{

		}

		public virtual void OnTriggerExit(Trigger trig)
		{

		}
		
		public virtual void OnImmediateCollisionEnter(CollisionChild col)
		{
			
		}
		
		public virtual void OnImmediateCollisionStay(CollisionChild col)
		{
			
		}

		public virtual void OnImmediateTriggerEnter(Trigger trig)
		{

		}

		public virtual void OnImmediateTriggerStay(Trigger trig)
		{

		}
	}
}

