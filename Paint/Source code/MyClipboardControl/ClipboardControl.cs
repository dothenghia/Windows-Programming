using MyShapes;
using System;

namespace MyClipboardControl
{
    public class ClipboardControl
    {
        public void Cut(List<IShape> drawnShapes, IShape selectShape, List<IShape> memory, Stack<IShape> buffer, List<int> position, Stack<int> positionBuffer)
        {
            IShape temp = (IShape)selectShape.Clone();
            memory.Add(temp);
            buffer.Push(temp);
            int index = drawnShapes.IndexOf(selectShape);
            if (index >= 0)
            {
                drawnShapes[index].CaptureState(IShape.ActionType.Delete);
                IShape.Action getPopped = drawnShapes[index].actionHistory[drawnShapes[index].actionHistory.Count - 1].Clone();
                getPopped.Type = IShape.ActionType.Delete;
                drawnShapes[index].actionBuffer.Push(getPopped);
                for (int i = 0; i < position.Count; i++)
                {
                    if (position[i] > index)
                    {
                        position[i] -= 1;
                    }
                    else if (position[i] == index)
                    {
                        position[i] = position.Max();
                        positionBuffer.Push(position.Max());
                        position.RemoveAt(position.Count - 1);
                        drawnShapes.RemoveAt(index);
                        return;
                    }
                }

                position.RemoveAt(position.Count - 1);
                positionBuffer.Push(drawnShapes.Count - 1);
                drawnShapes.RemoveAt(index);
                //for (int i = 0; i < position.Count; i++)
                //{
                //    if (position[i] > index)
                //    {
                //        position[i] -= 1;
                //    } else if (position[i] == index)
                //    {
                //        position[i] = drawnShapes.Count - 1;
                //    }
                //}
                //position.Add(drawnShapes.Count - 1);
                //drawnShapes.RemoveAt(index);
            }
        }

        public void Copy(IShape selectShape, List<IShape> memory)
        {
            IShape temp = (IShape)selectShape.Clone();
            memory.Add(temp);
        }

        public void Paste(List<IShape> drawnShapes, List<IShape> memory)
        {
            if(memory.Count > 0)
            {
                IShape temp = (IShape)memory[memory.Count - 1].Clone();
                drawnShapes.Add(temp);
                drawnShapes[drawnShapes.Count - 1].CaptureState(IShape.ActionType.Create);
            }
        }
    }

}
