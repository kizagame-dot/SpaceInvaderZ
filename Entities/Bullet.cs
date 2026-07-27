using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceInvaderZ.Core;

namespace SpaceInvaderZ.Entities;

public enum BulletOwner {Player , Enemy};

public class Bullet
{
    private Vector2 _position;
    private float _speed;
    private float _direction;
    public BulletOwner Owner {get;}
    public Rectangle Bounds => new Rectangle(
        (int)_position.X,
        (int)_position.Y,
        GameSettings.BulletWidth,
        GameSettings.BulletHeight
    );

    public Bullet(Vector2 startPosition, BulletOwner newOwner)
    {
        _position = startPosition;
        Owner = newOwner;

        _direction = newOwner == BulletOwner.Player ? -1f : 1f;
        _speed = GameSettings.BulletSpeed;
    }


    public bool IsActive => 
        _position.Y >= -GameSettings.BulletHeight &&
        _position.Y <= GameSettings.ScreenHeight;

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _position.Y += _direction * _speed * dt ;
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
    {
        Color color = Owner == BulletOwner.Player ? Color.White : Color.Red;
        spriteBatch.Draw(pixel,Bounds,color);
    }
}