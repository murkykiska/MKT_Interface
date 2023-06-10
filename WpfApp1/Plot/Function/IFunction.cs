using OpenTK.Mathematics;

namespace Plot.Function;

public interface IFunction
{
   void Draw(Color4 color, Box2 DrawArea);

}