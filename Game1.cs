using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceInvaderZ.Core;
using SpaceInvaderZ.Entities;

namespace SpaceInvaderZ;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _pixel;

    private Player _player;

    private Bullet? _playerBullet = null;
    private KeyboardState _previousKb;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = GameSettings.ScreenWidth;
        _graphics.PreferredBackBufferHeight = GameSettings.ScreenHeight;
        _graphics.ApplyChanges();

        _player = new Player();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _pixel = new Texture2D(GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });

    }

    protected override void Update(GameTime gameTime)
    {

                _player.Update(gameTime);

        KeyboardState currentKb = Keyboard.GetState();

        if (currentKb.IsKeyDown(Keys.Space) && _previousKb.IsKeyUp(Keys.Space))
        {

            if(_playerBullet == null && _player.CanFire())
            {
                _playerBullet = new Bullet(_player.FirePosition, BulletOwner.Player);
                _player.OnFire();
            }
        }

        _playerBullet?.Update(gameTime);

        if(_playerBullet != null && !_playerBullet.IsActive)
            _playerBullet = null;

        _previousKb = currentKb;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend
        );

        _player.Draw(_spriteBatch, _pixel);
        _playerBullet?.Draw(_spriteBatch, _pixel);


        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
