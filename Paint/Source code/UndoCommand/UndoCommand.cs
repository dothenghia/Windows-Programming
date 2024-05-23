using ICommand;
using MyRevisionControl;
using MyShapes;

namespace MyUndoCommand
{
    public class UndoCommand : Command
    {
        private readonly RevisionControl revisionControl;
        private readonly List<IShape> drawnShapes;
        private readonly Stack<IShape> buffer;
        private readonly List<int> position;
        private readonly List<IShape> memory;
        private readonly Stack<int> positionBuffer;
        private readonly int count;
        public UndoCommand(RevisionControl revisionControl, List<IShape> drawnShapes, Stack<IShape> buffer, List<IShape> memory, List<int> position, Stack<int> positionBuffer, int count)
        {
            this.revisionControl = revisionControl;
            this.drawnShapes = drawnShapes;
            this.buffer = buffer;
            this.position = position;
            this.memory = memory;
            this.positionBuffer = positionBuffer;
            this.count = count;
        }

        public void Execute()
        {
            revisionControl.Undo(drawnShapes, buffer, memory, position, positionBuffer);
        }

        public void Undo()
        {
            revisionControl.Redo(drawnShapes, buffer, position, positionBuffer, count);
        }
    }

}
