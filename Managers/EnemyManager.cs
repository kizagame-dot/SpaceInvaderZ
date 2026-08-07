using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using SpaceInvaderZ.Entities;
using SpaceInvaderZ.Core;
using System;

namespace SpaceInvaderZ.Managers;

public class EnemyManager
{
    public List<Enemy> Enemies{get;} = new List<Enemy>();
    private List<Bullet> _enemyBullets = new List<Bullet>();
    public IReadOnlyList<Bullet> EnemyBullets => _enemyBullets;
    private float _fireTimer = 0f;
    private Random _rng = new Random();
    private Vector2 _gridOffset = Vector2.Zero;
    private float _direction = 1f;
    private int _totalEnemies = GameSettings.EnemyRows * GameSettings.EnemyRows * GameSettings.EnemyCols;

    private int _gridWidth = ( GameSettings.EnemyCols - 1) * GameSettings.EnemySpacingX + GameSettings.EnemyWidth;
    private int _gridHeight =  ( GameSettings.EnemyRows - 1) * GameSettings.EnemySpacingY + GameSettings.EnemyRows;
    



    public bool ALLDead => Enemies.Count == 0 ;

    public bool ReachedPlayer => Enemies.Any(enemy => 
        enemy.Position.Y + GameSettings.EnemyHeight >= GameSettings.PlayerY);

    public EnemyManager()
    {
        SpawnGrid();
    }

    private void SpawnGrid()
    {
        Enemies.Clear();
        
        int startX =  ( GameSettings.ScreenWidth - _gridWidth ) / 2 ;
        int startY =  80 * (GameSettings.ScreenHeight / 600);

        for(int row = 0; row < GameSettings.EnemyRows; row++)
        {
            EnemyType type = row switch
            {
                0 or 1 => EnemyType.A,
                2 or 3 => EnemyType.B,
                _      => EnemyType.C
            };


            for(int col=0; col < GameSettings.EnemyCols; col++)
            {
                Vector2 pos = new Vector2(
                    startX + col * GameSettings.EnemySpacingX,
                    startY + row * GameSettings.EnemySpacingY
                );

                Enemies.Add(new Enemy(pos,type));
            }
        }
    }

    public void Update(GameTime gameTime)
    {

        if(Enemies.Count == 0) return ;

        float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;

        //Accelaration
        float speedFactor = 1f + (1f - (float)Enemies.Count / _totalEnemies) * 3f ;
        float speed  = GameSettings.EnemySpeed * speedFactor;

        foreach(var enemy in Enemies)
        {
            Vector2 pos = enemy.Position;
            pos.X   += (float) _direction * speed * dt;
            enemy.Position = pos;
        }
        
        //Edge of the grill
        float leftMost =  Enemies.Min(e => e.Position.X);
        float rightMost = Enemies.Max(e => e.Position.X + GameSettings.EnemyWidth );

        bool hitRight = rightMost >= GameSettings.ScreenWidth - 20 ;  
        bool hitLeft = leftMost <= 20;

        

        if(hitRight | hitLeft)
        {
            _direction *= -1f; 

            foreach(var enemy in Enemies)
            {
                Vector2 pos = enemy.Position;
                pos.Y += GameSettings.EnemyDropY;
                enemy.Position = pos;
            }
        }


        dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        _fireTimer -= dt;

        if (_fireTimer <= 0f && Enemies.Count > 0)
        {
            _fireTimer  = GameSettings.EnemyFireRate;
            TryFireEnemy();
        } 

        foreach(var bullet in _enemyBullets)
        {
            bullet.Update(gameTime);
        }

        _enemyBullets.RemoveAll(bullet => !bullet.IsActive);

    }


    public void TryFireEnemy()
    {
        List<Enemy> bottomEnemies = GetBottomEnemies();
        if(bottomEnemies.Count == 0 ) return;

        Enemy shooter = bottomEnemies[_rng.Next(bottomEnemies.Count)];

        Vector2 spawnPos = new Vector2(
            shooter.Position.X + GameSettings.EnemyWidth / 2f + GameSettings.BulletWidth / 2f,
            shooter.Position.Y + GameSettings.EnemyHeight
        );

        _enemyBullets.Add(new Bullet(spawnPos, BulletOwner.Enemy));
    }

    public List<Enemy> GetBottomEnemies()
    {   
        
        var bottomEnemies = new List<Enemy>();

        float startX =  (float) ( GameSettings.ScreenWidth - _gridWidth ) / 2f ;


        for (int col= 0; col < GameSettings.EnemyCols; col ++)
        {
            float colX = startX + col * GameSettings.EnemySpacingX;

            Enemy? bottomInCol = null;

            foreach(var enemy in Enemies)
            {
                if(Math.Abs(enemy.Position.X - colX) < 20)
                {
                    if(bottomInCol == null  || enemy.Position.Y > bottomInCol.Position.Y)
                        bottomInCol = enemy;
                }
            }

            if(bottomInCol != null)
            {
                bottomEnemies.Add(bottomInCol);
            }
        }

        return bottomEnemies;

    }




    

    public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
    {
        foreach(var enemy in Enemies)
        {
            enemy.Draw(spriteBatch,pixel);
        }

        foreach(var bullet in _enemyBullets)
        {
            bullet.Draw(spriteBatch,pixel);
        }
    }
}