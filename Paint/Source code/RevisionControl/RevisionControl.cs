using MyShapes;
using System;
using System.Reflection;

namespace MyRevisionControl
{
    public class RevisionControl
    {
        public void Undo(List<IShape>drawnShapes, Stack<IShape> buffer, List<IShape> memory, List<int> position, Stack<int> positionBuffer)
        {
            if (drawnShapes.Count == 0 && buffer.Count == 0)
                return;
            if(drawnShapes.Count != 0)
            {
                if (!drawnShapes[position[position.Count - 1]].undo())
                {
                    buffer.Push(drawnShapes[position[position.Count - 1]]);
                    drawnShapes.RemoveAt(position[position.Count - 1]);

                    for(int i = 0; i < position.Count; i++)
                    {
                        if(position[i] > position[position.Count - 1])
                        {
                            position[i] -= 1;
                        } else if(position[i] == position[position.Count - 1])
                        {
                            position[i] = position.Max();
                            positionBuffer.Push(position.Max());
                            position.RemoveAt(position.Count - 1);
                            return;
                        }
                    }
                    position.RemoveAt(position.Count - 1);
                    positionBuffer.Push(drawnShapes.Count - 1);
                } else
                {
                    positionBuffer.Push(position[position.Count - 1]);
                    position.RemoveAt(position.Count - 1);

                }
            } else if(memory.Count != 0) 
            {
                if (drawnShapes.Count == 0 && buffer.Count != 0 && buffer.Peek().startPoint == memory[memory.Count - 1].startPoint && buffer.Peek().endPoint == memory[memory.Count - 1].endPoint)
                {
                    drawnShapes.Add((IShape)buffer.Pop().Clone());
                    drawnShapes[drawnShapes.Count - 1].CaptureState(IShape.ActionType.Create);
                    IShape.Action getPopped = drawnShapes[drawnShapes.Count - 1].actionHistory[drawnShapes[drawnShapes.Count - 1].actionHistory.Count - 1].Clone();
                    drawnShapes[drawnShapes.Count - 1].actionBuffer.Push(getPopped);
                    memory.RemoveAt(memory.Count - 1);
                    position.Add(drawnShapes.Count - 1);
                }    
            }
        }

        public void Redo(List<IShape> drawnShapes, Stack<IShape> buffer, List<int> position, Stack<int> positionBuffer, int count)
        {
            if (drawnShapes.Count == 0 && buffer.Count == 0)
                return;
            if (positionBuffer.Count > 0)
            {
                position.Add(positionBuffer.Pop());

                if (drawnShapes.Count - 1 < position[position.Count - 1])
                {
                    drawnShapes.Add(buffer.Pop());
                    drawnShapes[drawnShapes.Count - 1].actionBuffer.Peek().Type = IShape.ActionType.Create;
                    drawnShapes[drawnShapes.Count - 1].actionHistory.Add(drawnShapes[drawnShapes.Count - 1].actionBuffer.Pop());
                }
                else
                {
                    drawnShapes[position[position.Count - 1]].redo();
                }
            }
            
        }
    }

}
