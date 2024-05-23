
namespace ICommand
{
    public interface Command
    {
        void Execute();
        void Undo();
    }
}
