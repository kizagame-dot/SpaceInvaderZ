using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceInvaderZ.Core;

namespace SpaceInvaderZ.Entities;


public enum EnemyType
{
    A = 30,
    B = 20,
    C = 10
}
public class Enemy
{
    public Vector2 Position { get; set;}
    public EnemyType Type {get;}
    public bool isAlive  {get; private set;}

    public Rectangle Bounds => new Rectangle(
        (int)Position.X,
        (int)Position.Y,
        GameSettings.EnemyWidth,
        GameSettings.EnemyHeight
    );

    public int Points => (int)Type;

    public Enemy(Vector2 position, EnemyType type)
    {
        Position = position;
        Type = type;
        isAlive = true;
    }

    public void Kill()
    {
        isAlive = false;
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
    {
        if(! isAlive) return;

        Color color = Type switch
        {
            EnemyType.A => Color.White,
            EnemyType.B => Color.Cyan,
            EnemyType.C => Color.Magenta,
            _           => Color.White,
        };



        if(Type == EnemyType.A)
        {
            spriteBatch.Draw(pixel,
                new Rectangle((int)Position.X - 4, (int)Position.Y + 4, 4, 8),
                color * 0.8f);
            spriteBatch.Draw(pixel,
                new Rectangle((int)Position.X + GameSettings.EnemyWidth, (int)Position.Y + 4, 4, 8),
                color * 0.8f);
        }
         else if (Type == EnemyType.B)
        {
        
            spriteBatch.Draw(pixel,
                new Rectangle((int)Position.X + 4, (int)Position.Y - 6, 4, 6),
                color * 0.8f);
            spriteBatch.Draw(pixel,
                new Rectangle((int)Position.X + GameSettings.EnemyWidth - 8, (int)Position.Y - 6, 4, 6),
                color * 0.8f);
        }


        spriteBatch.Draw(pixel,Bounds,color);
        
    }




}
