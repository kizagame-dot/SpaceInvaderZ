using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceInvaderZ.Core;

namespace SpaceInvaderZ.Entities;

public class Player
{
    //Position x in float for more accuracy
    private float _x;
    private float _fireCooldown = 0f;

    public Rectangle Bounds => new Rectangle(
        (int) _x,
        GameSettings.PlayerY,
        GameSettings.PlayerWidth,
        GameSettings.PlayerHeight
    );


    public bool CanFire => _fireCooldown <= 0f;

    public bool IsAlive {get; private set;} = true;

    public Player()
    {
        _x = GameSettings.ScreenWidth / 2f - GameSettings.PlayerWidth / 2f;
    }

    public void OnFire()
    {
        _fireCooldown = GameSettings.PlayerFireRate;
    }

    public void Kill()
    {
        IsAlive = false;
    }

    public void Respawn()
    {
        IsAlive = true;
        _x = GameSettings.ScreenWidth / 2f - GameSettings.PlayerWidth / 2f;
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        KeyboardState kb = Keyboard.GetState();

        if(kb.IsKeyDown(Keys.Left) || kb.IsKeyDown(Keys.Q))
            _x -= GameSettings.PlayerSpeed * dt;

        if(kb.IsKeyDown(Keys.Right) || kb.IsKeyDown(Keys.D))
            _x += GameSettings.PlayerSpeed * dt;

        _x = MathHelper.Clamp(_x,0,GameSettings.ScreenWidth - GameSettings.PlayerWidth);

        if (_fireCooldown > 0f)
            _fireCooldown -= dt;


    }

    public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
    {
        if(!IsAlive) return;

        spriteBatch.Draw(
            pixel,
            new Rectangle((int)_x, GameSettings.PlayerY, GameSettings.PlayerWidth, GameSettings.PlayerHeight),
            Color.LimeGreen);
        
        spriteBatch.Draw(
            pixel,
            new Rectangle(
                (int)_x + GameSettings.PlayerWidth / 2 - 2,
                GameSettings.PlayerY - 6,
                4,8
                ),
            Color.LimeGreen);
    }
}
