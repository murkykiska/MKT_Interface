using OpenTK.Mathematics;

namespace Plot.Function;

public interface IFunction
{
   void Draw(Color4 color, Vector2 center, Vector2 scale);
   Box2 GetDomain();

}