
using ICommand;

namespace MyToolbarCommand
{
    public class ToolBarCommand
    {
        private readonly Command Cut;
        private readonly Command Undo;
        public ToolBarCommand(Command Cut, Command Undo)
        {
            this.Cut = Cut;
            this.Undo = Undo;
        }
        public ToolBarCommand(Command Undo)
        {
            this.Undo = Undo;
        }
        public void Toolbar_Copy()
        {
            Cut.Execute();
        }
        public void Toolbar_Cut()
        {
            Cut.Execute();
        }
        public void Toolbar_Paste()
        {
            Cut.Undo();
        }

        public void Toolbar_Undo()
        {
            Undo.Execute();
        }
        public void Toolbar_Redo()
        {
            Undo.Undo();
        }
    }

}
