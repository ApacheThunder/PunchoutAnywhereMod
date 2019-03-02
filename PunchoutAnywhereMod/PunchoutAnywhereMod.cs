namespace PunchoutAnywhereMod {
	public class PunchoutAnywhereMod : ETGModule {
		public override void Init() { }
		public override void Start() { ETGModMainBehaviour.Instance.gameObject.AddComponent<PunchoutInit>(); }
		public override void Exit() { }
	}
}

