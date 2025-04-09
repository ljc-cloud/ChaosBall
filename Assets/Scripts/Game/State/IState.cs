namespace ChaosBall.Game.State
{
    public interface IState
    {
        void Enter();
        void Exit();
        void Update();
    }
}