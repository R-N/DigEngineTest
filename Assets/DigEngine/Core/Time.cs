using System;
namespace DigEngine
{
	public class Time : DigObject
	{
		private float _actualDeltaTime = 0.03125f;
		private float _timeScale = 1;
		private float _deltaTime = 0.03125f;

		private float __actualDeltaTime = 0.03125f;
		private float __timeScale = 1;


		public float actualDeltaTime
		{
			get
			{
				return _actualDeltaTime;
			}
			set
			{
				__actualDeltaTime = value;
			}
		}
		public float timeScale
		{
			get
			{
				return _timeScale;
			}
			set
			{
				__timeScale = value;
			}
		}
		public float deltaTime
		{
			get
			{
				return _deltaTime;
			}
		}

		public float time = 0;
		public float actualTime = 0;

		public Time(GameInstance gameInstance) : base(gameInstance)
		{
		}
		public override void Start1()
		{
			SetDeltaTime();
		}
		private void SetDeltaTime()
		{
			_actualDeltaTime = __actualDeltaTime;
			_timeScale = __timeScale;
			_deltaTime = _actualDeltaTime * _timeScale;
		}
		public void PreUpdate(){
			SetDeltaTime();
			time += deltaTime;
			actualTime += actualDeltaTime;
			++timeStamp;
		}

		public void PostUpdate()
		{
		}
		

	}
}
