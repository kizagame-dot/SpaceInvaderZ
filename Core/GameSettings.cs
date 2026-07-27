using System;

namespace SpaceInvaderZ.Core;

public static class GameSettings
{
    // Screen
    public const int ScreenWidth = 800;
    public const int ScreenHeight = 600;

    // Global
    public const int MaxLife = 3;
    public const int WinScore = 0;

    // Player
    public const float PlayerSpeed = 300f;
    public const int PlayerWidth = 40;
    public const int PlayerHeight = 24;
    public const int PlayerY = 540;
    public const float PlayerFireRate = 0.3f;


    // Enemies
    public const int EnemyCols = 11;
    public const int EnemyRows = 5;
    public const int EnemyWidth = 32;
    public const int EnemyHeight = 24;
    public const int EnemySpacingX = 48;
    public const int EnemySpacingY = 48;
    public const float EnemySpeed = 30f;
    public const float EnemyDropY = 20f;

    // Projectiles
    public const int BulletWidth = 4;
    public const int BulletHeight = 12;
    public const float BulletSpeed = 500f;
    public const float EnemyFireRate = 1.5f;




}
