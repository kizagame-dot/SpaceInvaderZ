# 🚀 Plan Complet — Space Invaders + Leaderboard Django
> Référence de travail personnelle · Suite du projet Pong MonoGame C#

---

## 🧭 Comment utiliser ce document

- **Reviens ici avant chaque session** pour savoir où tu en es
- **Coche les cases** au fur et à mesure (`[x]`)
- Chaque étape a : théorie → code annoté → exercice de validation
- Ne passe **jamais à l'étape suivante** sans avoir validé la précédente

---

## 🗺️ Vue d'ensemble du projet

```
PHASE 1 — Space Invaders local
    Étape 1  → Setup + vaisseau joueur
    Étape 2  → Projectiles joueur
    Étape 3  → Grille d'ennemis
    Étape 4  → Mouvement des ennemis
    Étape 5  → Tir ennemis
    Étape 6  → Collisions complètes
    Étape 7  → Vies, score, états de jeu
    Étape 8  → Sons, particules, polish

PHASE 2 — API Django
    Étape 9  → Modèle Score + API REST
    Étape 10 → HttpClient C# → POST score
    Étape 11 → GET leaderboard → affichage in-game

PHASE 3 — Finitions
    Étape 12 → Entrée pseudo (saisie clavier)
    Étape 13 → Écran leaderboard animé
    Étape 14 → Shaders (réutiliser ce qu'on sait)
```

---

## ✅ Progression globale

- [ ] Étape 1  — Setup + vaisseau joueur
- [ ] Étape 2  — Projectiles joueur
- [ ] Étape 3  — Grille d'ennemis
- [ ] Étape 4  — Mouvement des ennemis
- [ ] Étape 5  — Tir ennemis
- [ ] Étape 6  — Collisions complètes
- [ ] Étape 7  — Vies, score, états de jeu
- [ ] Étape 8  — Sons, particules, polish
- [ ] Étape 9  — API Django (modèle + endpoints)
- [ ] Étape 10 — HttpClient C# POST score
- [ ] Étape 11 — GET leaderboard in-game
- [ ] Étape 12 — Saisie pseudo clavier
- [ ] Étape 13 — Écran leaderboard animé
- [ ] Étape 14 — Shaders

---

## 🏗️ Architecture cible

```
SpaceInvaders/
├── Game1.cs
├── Core/
│   ├── GameState.cs
│   └── GameSettings.cs          ← constantes globales
├── Entities/
│   ├── Player.cs                ← vaisseau joueur
│   ├── Enemy.cs                 ← un ennemi individuel
│   ├── Bullet.cs                ← projectile (joueur ou ennemi)
│   └── Shield.cs                ← bouclier destructible
├── Managers/
│   ├── EnemyManager.cs          ← grille + mouvement + tir
│   ├── BulletManager.cs         ← tous les projectiles actifs
│   ├── ScoreManager.cs          ← score local + vies
│   ├── SoundManager.cs
│   ├── FontManager.cs
│   └── ApiManager.cs            ← HTTP vers Django
├── Screens/
│   ├── MenuScreen.cs
│   ├── GameScreen.cs
│   ├── GameOverScreen.cs
│   └── LeaderboardScreen.cs
└── Content/
    ├── Sprites/
    └── Shaders/
```

---

---

# PHASE 1 — Space Invaders Local

---

# ÉTAPE 1 — Setup + Vaisseau Joueur

## 🎯 Objectif
Créer le projet, mettre en place l'architecture de base, afficher et déplacer le vaisseau du joueur horizontalement en bas de l'écran.

## 📚 Théorie

### Différences avec Pong

```
Pong                          Space Invaders
────────────────────────      ────────────────────────
Mouvement vertical            Mouvement horizontal
2 joueurs                     1 joueur vs ennemis
Physique de balle             Projectiles en ligne droite
Pas de limite de tir          Limite : 1 balle à la fois (classic)
```

### GameSettings — les constantes globales
Plutôt que d'avoir des magic numbers partout, on regroupe toutes les constantes dans une classe statique :

```csharp
// Core/GameSettings.cs
namespace SpaceInvaders.Core;

public static class GameSettings
{
    // Écran
    public const int ScreenWidth  = 800;
    public const int ScreenHeight = 600;

    // Joueur
    public const float PlayerSpeed    = 300f;
    public const int   PlayerWidth    = 40;
    public const int   PlayerHeight   = 24;
    public const int   PlayerY        = 540;  // position Y fixe en bas
    public const float PlayerFireRate = 0.5f; // secondes entre deux tirs

    // Ennemis
    public const int   EnemyCols      = 11;
    public const int   EnemyRows      = 5;
    public const int   EnemyWidth     = 32;
    public const int   EnemyHeight    = 24;
    public const int   EnemySpacingX  = 48;
    public const int   EnemySpacingY  = 48;
    public const float EnemySpeed     = 30f;  // vitesse initiale
    public const float EnemyDropY     = 20f;  // descend de 20px à chaque rebord

    // Projectiles
    public const int   BulletWidth    = 4;
    public const int   BulletHeight   = 12;
    public const float BulletSpeed    = 500f;
    public const float EnemyFireRate  = 1.5f; // secondes entre tirs ennemis
}
```

### Pourquoi `static` pour GameSettings ?
Une classe statique ne peut pas être instanciée. C'est parfait pour les constantes — on y accède directement sans créer d'objet :

```csharp
// Avec classe statique :
float speed = GameSettings.PlayerSpeed; // ← direct, propre ✅

// Sans classe statique (à éviter) :
float speed = 300f; // ← magic number ❌
```

### Le vaisseau joueur — mouvement horizontal uniquement
Contrairement à Pong où les raquettes bougeaient verticalement, le joueur de Space Invaders se déplace seulement sur l'axe X, toujours à la même hauteur Y.

```
Y fixe →  [  ▲  ]  ← vaisseau
          ════════════════════  ← sol
```

## 💻 Code

### `Core/GameSettings.cs`
```csharp
namespace SpaceInvaders.Core;

public static class GameSettings
{
    public const int   ScreenWidth    = 800;
    public const int   ScreenHeight   = 600;
    public const float PlayerSpeed    = 300f;
    public const int   PlayerWidth    = 40;
    public const int   PlayerHeight   = 24;
    public const int   PlayerY        = 540;
    public const float PlayerFireRate = 0.5f;
    public const int   EnemyCols      = 11;
    public const int   EnemyRows      = 5;
    public const int   EnemyWidth     = 32;
    public const int   EnemyHeight    = 24;
    public const int   EnemySpacingX  = 48;
    public const int   EnemySpacingY  = 48;
    public const float EnemyBaseSpeed = 30f;
    public const float EnemyDropY     = 20f;
    public const int   BulletWidth    = 4;
    public const int   BulletHeight   = 12;
    public const float BulletSpeed    = 500f;
    public const float EnemyFireRate  = 1.5f;
    public const int   MaxLives       = 3;
    public const int   WinScore       = 0; // 0 = pas de score max
}
```

### `Core/GameState.cs`
```csharp
namespace SpaceInvaders.Core;

public enum GameState
{
    Menu,           // Écran d'accueil
    Playing,        // Partie en cours
    PlayerDead,     // Animation de mort du joueur (pause courte)
    LevelComplete,  // Vague terminée → prochaine vague
    GameOver,       // Plus de vies
    Victory,        // Tous les ennemis détruits
    Leaderboard,    // Affichage du top 10
    EnterName,      // Saisie du pseudo
}
```

### `Entities/Player.cs`
```csharp
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceInvaders.Core;

namespace SpaceInvaders.Entities;

public class Player
{
    // Position en float pour la précision
    private float _x;

    // Hitbox publique (calculée depuis _x)
    public Rectangle Bounds => new Rectangle(
        (int)_x,
        GameSettings.PlayerY,
        GameSettings.PlayerWidth,
        GameSettings.PlayerHeight
    );

    // Cooldown entre deux tirs
    private float _fireCooldown = 0f;

    // Le joueur peut-il tirer ?
    public bool CanFire => _fireCooldown <= 0f;

    // Est-il vivant ?
    public bool IsAlive { get; private set; } = true;

    public Player()
    {
        // Centrer le joueur horizontalement
        _x = GameSettings.ScreenWidth / 2f - GameSettings.PlayerWidth / 2f;
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        KeyboardState kb = Keyboard.GetState();

        // Mouvement gauche / droite
        if (kb.IsKeyDown(Keys.Left) || kb.IsKeyDown(Keys.Q))
            _x -= GameSettings.PlayerSpeed * dt;

        if (kb.IsKeyDown(Keys.Right) || kb.IsKeyDown(Keys.D))
            _x += GameSettings.PlayerSpeed * dt;

        // Empêcher de sortir de l'écran
        _x = MathHelper.Clamp(_x, 0, GameSettings.ScreenWidth - GameSettings.PlayerWidth);

        // Réduire le cooldown de tir
        if (_fireCooldown > 0f)
            _fireCooldown -= dt;
    }

    // Appelé quand le joueur tire — réinitialise le cooldown
    public void OnFire()
    {
        _fireCooldown = GameSettings.PlayerFireRate;
    }

    // Appelé quand le joueur est touché
    public void Kill()
    {
        IsAlive = false;
    }

    public void Respawn()
    {
        IsAlive = true;
        _x = GameSettings.ScreenWidth / 2f - GameSettings.PlayerWidth / 2f;
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
    {
        if (!IsAlive) return;

        // Corps du vaisseau — rectangle principal
        spriteBatch.Draw(pixel,
            new Rectangle((int)_x, GameSettings.PlayerY,
                          GameSettings.PlayerWidth, GameSettings.PlayerHeight),
            Color.LimeGreen);

        // Canon — petit rectangle au centre en haut
        spriteBatch.Draw(pixel,
            new Rectangle((int)_x + GameSettings.PlayerWidth / 2 - 2,
                          GameSettings.PlayerY - 6, 4, 8),
            Color.LimeGreen);
    }
}
```

### `Game1.cs` — version minimale de départ
```csharp
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceInvaders.Core;
using SpaceInvaders.Entities;

namespace SpaceInvaders;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _pixel;

    private Player _player;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth  = GameSettings.ScreenWidth;
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
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        _player.Draw(_spriteBatch, _pixel);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
```

## ✅ Validation de l'étape 1
- [ ] Le projet compile et une fenêtre noire s'ouvre
- [ ] Le vaisseau vert apparaît en bas de l'écran
- [ ] Flèches gauche/droite (ou Q/D) le déplacent
- [ ] Il ne sort pas de l'écran
- [ ] `GameSettings` regroupe toutes les constantes

---

---

# ÉTAPE 2 — Projectiles Joueur

## 🎯 Objectif
Le joueur peut tirer avec Espace. Un seul projectile à la fois (règle classique Space Invaders). Le projectile monte et disparaît en haut de l'écran.

## 📚 Théorie

### Pourquoi un seul projectile à la fois ?
C'est la règle du Space Invaders original (1978). Ça force le joueur à viser avec précision. Techniquement, c'est aussi plus simple : pas besoin de gérer une liste, juste un seul objet nullable.

```
// Au lieu de List<Bullet> (pour les ennemis, plus tard)
Bullet? _playerBullet = null;  // null = pas de balle en jeu
```

### Nullable — le `?` en C#
```csharp
Bullet? _bullet = null;  // peut être null ou un Bullet

// Vérifier avant d'utiliser
if (_bullet != null)
{
    _bullet.Update(gameTime);
}

// Syntaxe moderne équivalente
_bullet?.Update(gameTime); // ← ne fait rien si null
```

### Détecter une pression unique (rappel de Pong)
```csharp
private KeyboardState _previousKb;

// Dans Update() :
KeyboardState currentKb = Keyboard.GetState();

if (currentKb.IsKeyDown(Keys.Space) && _previousKb.IsKeyUp(Keys.Space))
{
    // Vient d'appuyer → tirer
}

_previousKb = currentKb; // toujours à la fin
```

## 💻 Code

### `Entities/Bullet.cs`
```csharp
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceInvaders.Core;

namespace SpaceInvaders.Entities;

public enum BulletOwner { Player, Enemy }

public class Bullet
{
    private Vector2 _position;
    private float _speed;

    // Direction : -1 = vers le haut (joueur), +1 = vers le bas (ennemi)
    private float _direction;

    public BulletOwner Owner { get; }

    // Rectangle de collision
    public Rectangle Bounds => new Rectangle(
        (int)_position.X,
        (int)_position.Y,
        GameSettings.BulletWidth,
        GameSettings.BulletHeight
    );

    // Est-ce que la balle est encore à l'écran ?
    public bool IsActive =>
        _position.Y > -GameSettings.BulletHeight &&
        _position.Y < GameSettings.ScreenHeight;

    public Bullet(Vector2 startPosition, BulletOwner owner)
    {
        _position = startPosition;
        Owner      = owner;

        // Joueur tire vers le haut (-Y), ennemi vers le bas (+Y)
        _direction = owner == BulletOwner.Player ? -1f : 1f;
        _speed     = GameSettings.BulletSpeed;
    }

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _position.Y += _direction * _speed * dt;
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
    {
        Color color = Owner == BulletOwner.Player ? Color.White : Color.Red;
        spriteBatch.Draw(pixel, Bounds, color);
    }
}
```

### Modifications dans `Player.cs` — position de spawn du tir
```csharp
// Ajouter cette propriété publique dans Player.cs
// Retourne le point de départ du projectile (canon du vaisseau)
public Vector2 FirePosition => new Vector2(
    _x + GameSettings.PlayerWidth / 2f - GameSettings.BulletWidth / 2f,
    GameSettings.PlayerY - GameSettings.BulletHeight
);
```

### Modifications dans `GameScreen.cs` (ou `Game1.cs` pour l'instant)
```csharp
// Déclaration
private Bullet? _playerBullet = null;
private KeyboardState _previousKb;

// Dans Update() :
KeyboardState currentKb = Keyboard.GetState();

// Tir joueur
if (currentKb.IsKeyDown(Keys.Space) && _previousKb.IsKeyUp(Keys.Space))
{
    // Tirer seulement si aucune balle en jeu ET cooldown terminé
    if (_playerBullet == null && _player.CanFire)
    {
        _playerBullet = new Bullet(_player.FirePosition, BulletOwner.Player);
        _player.OnFire();
    }
}

// Mettre à jour la balle
_playerBullet?.Update(gameTime);

// Supprimer si hors écran
if (_playerBullet != null && !_playerBullet.IsActive)
    _playerBullet = null;

_previousKb = currentKb;

// Dans Draw() :
_playerBullet?.Draw(_spriteBatch, _pixel);
```

## ✅ Validation de l'étape 2
- [ ] Appuyer sur Espace fait apparaître un projectile blanc
- [ ] Le projectile monte et disparaît en haut
- [ ] On ne peut pas tirer si une balle est déjà en vol
- [ ] Le cooldown empêche le spam

---

---

# ÉTAPE 3 — Grille d'Ennemis

## 🎯 Objectif
Afficher la grille de 5×11 ennemis. 3 types d'ennemis selon la ligne. Aucun mouvement pour l'instant.

## 📚 Théorie

### La grille — tableau 2D
```
         col 0  col 1  col 2 ... col 10
ligne 0:   👾    👾    👾  ...   👾   (type A — 30 pts)
ligne 1:   👾    👾    👾  ...   👾   (type A — 30 pts)
ligne 2:   👽    👽    👽  ...   👽   (type B — 20 pts)
ligne 3:   👽    👽    👽  ...   👽   (type B — 20 pts)
ligne 4:   🛸    🛸    🛸  ...   🛸   (type C — 10 pts)
```

Les ennemis du bas valent moins mais sont les premiers à être touchés.

### Structure de données — `List<Enemy>` plutôt que tableau 2D
```csharp
// Tableau 2D : difficile à gérer quand les ennemis meurent
Enemy[,] enemies = new Enemy[5, 11]; // ← cases vides à gérer

// Liste plate : simple, on supprime directement
List<Enemy> _enemies = new List<Enemy>();
// → quand un ennemi meurt : _enemies.Remove(enemy)
```

## 💻 Code

### `Entities/Enemy.cs`
```csharp
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceInvaders.Core;

namespace SpaceInvaders.Entities;

public enum EnemyType
{
    A = 30,  // valeur = points rapportés
    B = 20,
    C = 10
}

public class Enemy
{
    // Position en float pour les calculs de mouvement
    public Vector2 Position { get; set; }
    public EnemyType Type   { get; }
    public bool IsAlive     { get; private set; } = true;

    public Rectangle Bounds => new Rectangle(
        (int)Position.X,
        (int)Position.Y,
        GameSettings.EnemyWidth,
        GameSettings.EnemyHeight
    );

    public int Points => (int)Type; // les points = la valeur de l'enum

    public Enemy(Vector2 position, EnemyType type)
    {
        Position = position;
        Type     = type;
    }

    public void Kill()
    {
        IsAlive = false;
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
    {
        if (!IsAlive) return;

        // Couleur selon le type
        Color color = Type switch
        {
            EnemyType.A => Color.White,
            EnemyType.B => Color.Cyan,
            EnemyType.C => Color.Magenta,
            _           => Color.White
        };

        // Corps principal
        spriteBatch.Draw(pixel, Bounds, color);

        // Petites pattes selon le type (visuel simple)
        if (Type == EnemyType.A)
        {
            // Pattes en haut
            spriteBatch.Draw(pixel,
                new Rectangle((int)Position.X - 4, (int)Position.Y + 4, 4, 8),
                color * 0.8f);
            spriteBatch.Draw(pixel,
                new Rectangle((int)Position.X + GameSettings.EnemyWidth, (int)Position.Y + 4, 4, 8),
                color * 0.8f);
        }
        else if (Type == EnemyType.B)
        {
            // Antennes
            spriteBatch.Draw(pixel,
                new Rectangle((int)Position.X + 4, (int)Position.Y - 6, 4, 6),
                color * 0.8f);
            spriteBatch.Draw(pixel,
                new Rectangle((int)Position.X + GameSettings.EnemyWidth - 8, (int)Position.Y - 6, 4, 6),
                color * 0.8f);
        }
    }
}
```

### `Managers/EnemyManager.cs` — initialisation uniquement
```csharp
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceInvaders.Core;
using SpaceInvaders.Entities;

namespace SpaceInvaders.Managers;

public class EnemyManager
{
    public List<Enemy> Enemies { get; } = new List<Enemy>();

    // Offset de la grille entière (pour le mouvement)
    private Vector2 _gridOffset = Vector2.Zero;

    public EnemyManager()
    {
        SpawnGrid();
    }

    private void SpawnGrid()
    {
        Enemies.Clear();

        // Point de départ de la grille
        int startX = 80;
        int startY = 80;

        for (int row = 0; row < GameSettings.EnemyRows; row++)
        {
            // Déterminer le type selon la ligne
            EnemyType type = row switch
            {
                0 or 1 => EnemyType.A,  // 2 premières lignes
                2 or 3 => EnemyType.B,  // 2 lignes du milieu
                _      => EnemyType.C   // dernière ligne
            };

            for (int col = 0; col < GameSettings.EnemyCols; col++)
            {
                Vector2 pos = new Vector2(
                    startX + col * GameSettings.EnemySpacingX,
                    startY + row * GameSettings.EnemySpacingY
                );

                Enemies.Add(new Enemy(pos, type));
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
    {
        foreach (var enemy in Enemies)
        {
            enemy.Draw(spriteBatch, pixel);
        }
    }
}
```

## ✅ Validation de l'étape 3
- [ ] 55 ennemis apparaissent en grille (5 lignes × 11 colonnes)
- [ ] 3 couleurs différentes selon les lignes
- [ ] Les ennemis ne bougent pas encore

---

---

# ÉTAPE 4 — Mouvement des Ennemis

## 🎯 Objectif
La grille se déplace latéralement. Quand elle touche un bord, elle descend et change de direction. Plus il y a peu d'ennemis restants, plus ils vont vite.

## 📚 Théorie

### Le pattern de mouvement Space Invaders

```
→ → → → → → → (touche le bord droit)
                ↓ (descend de EnemyDropY)
← ← ← ← ← ← ← (touche le bord gauche)
                ↓ (descend de EnemyDropY)
→ → → → → → →
```

### Accélération progressive
Plus il y a peu d'ennemis restants, plus ils vont vite :

```csharp
// Speed factor : plus il y a peu d'ennemis, plus c'est rapide
float speedFactor = 1f + (1f - (float)Enemies.Count / TotalEnemies) * 3f;
// 55 ennemis → speedFactor = 1.0 (vitesse normale)
// 27 ennemis → speedFactor = 2.5
// 1 ennemi   → speedFactor = 4.0  (très rapide !)
```

### Détecter si la grille touche un bord
On calcule le rectangle englobant de tous les ennemis vivants :

```csharp
// Trouver les limites gauche et droite de la grille
float leftmost  = Enemies.Min(e => e.Position.X);
float rightmost = Enemies.Max(e => e.Position.X + GameSettings.EnemyWidth);

if (rightmost >= GameSettings.ScreenWidth - 20)
    // touche le bord droit → inverser et descendre
if (leftmost <= 20)
    // touche le bord gauche → inverser et descendre
```

## 💻 Code — ajout dans `EnemyManager.cs`

```csharp
public class EnemyManager
{
    // ... (code de l'étape 3)

    private float _direction    = 1f;  // 1 = droite, -1 = gauche
    private int   _totalEnemies = GameSettings.EnemyRows * GameSettings.EnemyCols;

    // Accès depuis GameScreen pour vérifier si tous les ennemis sont morts
    public bool AllDead => Enemies.Count == 0;

    // Accès pour vérifier si les ennemis ont atteint le joueur
    public bool ReachedPlayer => Enemies.Any(e =>
        e.Position.Y + GameSettings.EnemyHeight >= GameSettings.PlayerY);

    public void Update(GameTime gameTime)
    {
        if (Enemies.Count == 0) return;

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Accélération : plus peu d'ennemis → plus rapide
        float speedFactor = 1f + (1f - (float)Enemies.Count / _totalEnemies) * 3f;
        float speed       = GameSettings.EnemyBaseSpeed * speedFactor;

        // Déplacer tous les ennemis horizontalement
        foreach (var enemy in Enemies)
        {
            Vector2 pos = enemy.Position;
            pos.X += _direction * speed * dt;
            enemy.Position = pos;
        }

        // Vérifier les bords
        float leftmost  = Enemies.Min(e => e.Position.X);
        float rightmost = Enemies.Max(e => e.Position.X + GameSettings.EnemyWidth);

        bool hitRight = rightmost >= GameSettings.ScreenWidth - 20;
        bool hitLeft  = leftmost  <= 20;

        if (hitRight || hitLeft)
        {
            // Inverser la direction
            _direction *= -1f;

            // Faire descendre toute la grille
            foreach (var enemy in Enemies)
            {
                Vector2 pos = enemy.Position;
                pos.Y += GameSettings.EnemyDropY;
                enemy.Position = pos;
            }
        }
    }
}
```

## ✅ Validation de l'étape 4
- [ ] La grille se déplace vers la droite
- [ ] Elle rebondit sur les bords et descend
- [ ] Elle s'accélère quand il y a moins d'ennemis
- [ ] `AllDead` et `ReachedPlayer` sont accessibles depuis GameScreen

---

---

# ÉTAPE 5 — Tir des Ennemis

## 🎯 Objectif
Les ennemis tirent aléatoirement vers le bas. Plusieurs balles ennemies peuvent être en jeu simultanément.

## 📚 Théorie

### Différence avec le tir joueur

```
Joueur   → 1 seule balle    → Bullet? _playerBullet
Ennemis  → N balles         → List<Bullet> _enemyBullets
```

### Quel ennemi tire ?
Dans le Space Invaders classique, seuls les **ennemis en bas de chaque colonne** peuvent tirer (les premiers à voir le joueur). On choisit ensuite aléatoirement parmi eux.

```csharp
// Trouver les ennemis en bas de chaque colonne
// = l'ennemi avec le Y le plus grand dans chaque colonne
var shooters = GetBottomEnemies(); // List<Enemy>

// Choisir un au hasard
Enemy shooter = shooters[_rng.Next(shooters.Count)];
```

## 💻 Code — ajout dans `EnemyManager.cs`

```csharp
public class EnemyManager
{
    // ... (code des étapes précédentes)

    private List<Bullet> _enemyBullets = new List<Bullet>();
    private float _fireTimer = 0f;
    private Random _rng = new Random();

    // Accès depuis GameScreen pour les collisions
    public IReadOnlyList<Bullet> EnemyBullets => _enemyBullets;

    public void Update(GameTime gameTime)
    {
        // ... (mouvement de l'étape 4)

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Timer de tir ennemi
        _fireTimer -= dt;
        if (_fireTimer <= 0f && Enemies.Count > 0)
        {
            _fireTimer = GameSettings.EnemyFireRate;
            TryFireEnemy();
        }

        // Mettre à jour les balles ennemies
        foreach (var bullet in _enemyBullets)
            bullet.Update(gameTime);

        // Supprimer les balles hors écran
        _enemyBullets.RemoveAll(b => !b.IsActive);
    }

    private void TryFireEnemy()
    {
        // Trouver les ennemis en bas de chaque colonne
        List<Enemy> bottomEnemies = GetBottomEnemies();
        if (bottomEnemies.Count == 0) return;

        // Choisir un ennemi au hasard
        Enemy shooter = bottomEnemies[_rng.Next(bottomEnemies.Count)];

        // Créer une balle depuis le centre bas de cet ennemi
        Vector2 spawnPos = new Vector2(
            shooter.Position.X + GameSettings.EnemyWidth / 2f - GameSettings.BulletWidth / 2f,
            shooter.Position.Y + GameSettings.EnemyHeight
        );

        _enemyBullets.Add(new Bullet(spawnPos, BulletOwner.Enemy));
    }

    private List<Enemy> GetBottomEnemies()
    {
        // Pour chaque colonne, trouver l'ennemi le plus bas
        var bottomEnemies = new List<Enemy>();

        // Grouper par colonne (X approximatif)
        // On utilise le X de départ pour identifier la colonne
        for (int col = 0; col < GameSettings.EnemyCols; col++)
        {
            // X approximatif de cette colonne
            float colX = 80 + col * GameSettings.EnemySpacingX;

            // Trouver l'ennemi le plus bas dans cette colonne
            Enemy? bottomInCol = null;
            foreach (var e in Enemies)
            {
                // Tolérance de ±10px pour identifier la colonne
                if (Math.Abs(e.Position.X - colX) < 30)
                {
                    if (bottomInCol == null || e.Position.Y > bottomInCol.Position.Y)
                        bottomInCol = e;
                }
            }

            if (bottomInCol != null)
                bottomEnemies.Add(bottomInCol);
        }

        return bottomEnemies;
    }

    public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
    {
        foreach (var enemy in Enemies)
            enemy.Draw(spriteBatch, pixel);

        foreach (var bullet in _enemyBullets)
            bullet.Draw(spriteBatch, pixel);
    }
}
```

## ✅ Validation de l'étape 5
- [ ] Les ennemis tirent périodiquement
- [ ] Les balles rouges descendent vers le joueur
- [ ] Plusieurs balles peuvent être en jeu simultanément
- [ ] Les balles hors écran sont supprimées

---

---

# ÉTAPE 6 — Collisions Complètes

## 🎯 Objectif
Gérer toutes les collisions : balle joueur → ennemis, balles ennemies → joueur, balles → balles (annulation).

## 📚 Théorie

### Les 3 types de collisions

```
1. Balle joueur  ↔ Ennemi    → ennemi meurt, points ajoutés, balle supprimée
2. Balle ennemie ↔ Joueur    → joueur perd une vie, balle supprimée
3. Balle joueur  ↔ Balle ennemi → les deux s'annulent (optionnel, fun !)
```

### AABB — rappel de Pong
```csharp
// MonoGame a Rectangle.Intersects() intégré
if (bulletBounds.Intersects(enemyBounds))
{
    // collision !
}
```

## 💻 Code — `Managers/CollisionManager.cs`

```csharp
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SpaceInvaders.Core;
using SpaceInvaders.Entities;

namespace SpaceInvaders.Managers;

public class CollisionManager
{
    // Retourne le nombre de points gagnés ce frame
    public int CheckCollisions(
        Bullet?      playerBullet,
        EnemyManager enemyManager,
        Player       player,
        out bool     playerHit,
        out bool     playerBulletDestroyed)
    {
        int pointsGained         = 0;
        playerHit                = false;
        playerBulletDestroyed    = false;

        // 1. Balle joueur vs ennemis
        if (playerBullet != null)
        {
            foreach (var enemy in enemyManager.Enemies)
            {
                if (playerBullet.Bounds.Intersects(enemy.Bounds))
                {
                    pointsGained         += enemy.Points;
                    enemy.Kill();
                    playerBulletDestroyed = true;
                    break; // une balle ne peut tuer qu'un ennemi
                }
            }

            // Nettoyer les ennemis morts
            enemyManager.Enemies.RemoveAll(e => !e.IsAlive);
        }

        // 2. Balles ennemies vs joueur
        if (player.IsAlive)
        {
            foreach (var bullet in enemyManager.EnemyBullets)
            {
                if (bullet.Bounds.Intersects(player.Bounds))
                {
                    playerHit = true;
                    break;
                }
            }
        }

        return pointsGained;
    }
}
```

### Utilisation dans `GameScreen.Update()`

```csharp
// Après update des entités :
bool playerHit, bulletDestroyed;

int points = _collisionManager.CheckCollisions(
    _playerBullet,
    _enemyManager,
    _player,
    out playerHit,
    out bulletDestroyed
);

if (bulletDestroyed)
    _playerBullet = null;

if (points > 0)
    _scoreManager.AddPoints(points);

if (playerHit)
{
    _lives--;
    _player.Kill();
    if (_lives <= 0)
        _gameState = GameState.GameOver;
    else
        _gameState = GameState.PlayerDead;
}

if (_enemyManager.AllDead)
    _gameState = GameState.LevelComplete;

if (_enemyManager.ReachedPlayer)
    _gameState = GameState.GameOver;
```

## ✅ Validation de l'étape 6
- [ ] Tirer sur un ennemi le fait disparaître
- [ ] Les points s'ajoutent selon le type d'ennemi
- [ ] Une balle ennemie qui touche le joueur déclenche une mort
- [ ] Le jeu passe en GameOver si les ennemis atteignent le bas

---

---

# ÉTAPE 7 — Vies, Score et États de Jeu

## 🎯 Objectif
Système de vies (3), score affiché, tous les états gérés proprement, transitions entre vagues.

## 📚 Théorie

### Machine à états Space Invaders

```
Menu
  ↓ (Espace)
Playing ←──────────────────────────────┐
  ↓ (joueur touché)                    │
PlayerDead (pause 2 sec)               │
  ↓ (si vies > 0)                      │
Playing                                │
  ↓ (si vies = 0)                      │
GameOver                               │
  ↓ (Espace)                           │
EnterName                              │
  ↓ (Entrée)                           │
Leaderboard                            │
  ↓                                    │
Menu                                   │
                                       │
Playing ─── (tous ennemis morts) ──────┤
  ↓                                    │
LevelComplete (pause 2 sec)            │
  ↓                                    │
Playing (nouvelle vague + vitesse++) ──┘
```

### `ScoreManager` adapté pour Space Invaders

```csharp
public class ScoreManager
{
    public int Score     { get; private set; } = 0;
    public int HighScore { get; private set; } = 0;
    public int Lives     { get; private set; } = GameSettings.MaxLives;
    public int Wave      { get; private set; } = 1;

    public void AddPoints(int points)  => Score += points;
    public void LoseLife()             => Lives--;
    public void NextWave()             { Wave++; }
    public bool IsGameOver             => Lives <= 0;

    public void UpdateHighScore()
    {
        if (Score > HighScore) HighScore = Score;
    }

    public void Reset()
    {
        UpdateHighScore();
        Score = 0;
        Lives = GameSettings.MaxLives;
        Wave  = 1;
    }
}
```

### Affichage HUD

```csharp
// En haut à gauche : score
// En haut au centre : HI-SCORE
// En haut à droite : vague
// En bas à gauche : vies (icônes de vaisseau)

// Vies affichées comme des petites icônes
for (int i = 0; i < _scoreManager.Lives; i++)
{
    spriteBatch.Draw(_pixel,
        new Rectangle(10 + i * 30, 570, 20, 12),
        Color.LimeGreen);
}
```

## ✅ Validation de l'étape 7
- [ ] 3 vies au démarrage affichées en bas
- [ ] Le score s'incrémente selon le type d'ennemi détruit
- [ ] Le HI-SCORE est conservé entre les parties
- [ ] Mourir enlève une vie et respawn le joueur
- [ ] 0 vie → GameOver
- [ ] Tous ennemis détruits → LevelComplete → nouvelle vague

---

---

# ÉTAPE 8 — Sons, Particules et Polish

## 🎯 Objectif
Ajouter les sons procéduraux, explosions de particules, effets visuels. Réutiliser les systèmes du Pong.

## 📚 Théorie

### Sons Space Invaders
```
Tir joueur   → bip aigu court    (800 Hz, 0.05s)
Tir ennemi   → bip grave court   (200 Hz, 0.05s)
Mort ennemi  → explosion         (150 Hz, 0.2s, décroissant)
Mort joueur  → explosion longue  (100 Hz, 0.8s)
Marche ennemis → 4 sons cycliques (boom, boom, boom, boom)
```

### Marche des ennemis — le son iconique
```csharp
// 4 notes qui s'alternent en boucle
private int   _marchStep  = 0;
private float _marchTimer = 0f;
private float[] _marchFreqs = { 160f, 140f, 120f, 100f };

// Dans Update() :
_marchTimer -= dt;
if (_marchTimer <= 0f)
{
    _marchTimer = 0.5f / speedFactor; // s'accélère avec les ennemis
    _soundManager.Play("March" + _marchStep);
    _marchStep = (_marchStep + 1) % 4;
}
```

### Particules à la mort d'un ennemi
```csharp
// Réutiliser le ParticleManager du Pong directement
_particleManager.Emit(
    enemy.Position + new Vector2(GameSettings.EnemyWidth / 2f, GameSettings.EnemyHeight / 2f),
    count: 15,
    color: Color.Orange
);
```

## ✅ Validation de l'étape 8
- [ ] Son de tir au coup du joueur
- [ ] Son d'explosion quand un ennemi meurt
- [ ] Particules à la mort d'un ennemi
- [ ] Son de marche des ennemis qui s'accélère
- [ ] Son de mort du joueur

---

---

# PHASE 2 — API Django

---

# ÉTAPE 9 — API Django (Modèle + Endpoints)

## 🎯 Objectif
Créer un projet Django minimal avec un modèle `Score` et deux endpoints REST : POST pour soumettre un score, GET pour récupérer le top 10.

## 📚 Théorie

### Django REST Framework — rappel
```python
# Les deux endpoints dont on a besoin :
GET  /api/scores/     → retourne le top 10
POST /api/scores/     → soumet un nouveau score

# Format JSON :
# GET response :
[
    {"id": 1, "pseudo": "Kiza", "points": 4200, "wave": 5, "date": "2026-07-05"},
    {"id": 2, "pseudo": "Player2", "points": 3100, "wave": 3, "date": "2026-07-04"},
    ...
]

# POST body :
{"pseudo": "Kiza", "points": 4200, "wave": 5}
```

## 💻 Code Django

### `models.py`
```python
from django.db import models

class Score(models.Model):
    pseudo    = models.CharField(max_length=20)
    points    = models.IntegerField()
    wave      = models.IntegerField(default=1)
    date      = models.DateTimeField(auto_now_add=True)

    class Meta:
        ordering = ['-points']  # tri décroissant par score

    def __str__(self):
        return f"{self.pseudo} — {self.points} pts (vague {self.wave})"
```

### `serializers.py`
```python
from rest_framework import serializers
from .models import Score

class ScoreSerializer(serializers.ModelSerializer):
    class Meta:
        model  = Score
        fields = ['id', 'pseudo', 'points', 'wave', 'date']
        read_only_fields = ['id', 'date']
```

### `views.py`
```python
from rest_framework import generics
from .models import Score
from .serializers import ScoreSerializer

class ScoreListCreateView(generics.ListCreateAPIView):
    serializer_class = ScoreSerializer

    def get_queryset(self):
        # Retourner seulement le top 10
        return Score.objects.all()[:10]
```

### `urls.py`
```python
from django.urls import path
from .views import ScoreListCreateView

urlpatterns = [
    path('scores/', ScoreListCreateView.as_view(), name='scores'),
]
```

### Tester avec curl
```bash
# Soumettre un score
curl -X POST http://localhost:8000/api/scores/ \
     -H "Content-Type: application/json" \
     -d '{"pseudo": "Kiza", "points": 4200, "wave": 5}'

# Récupérer le top 10
curl http://localhost:8000/api/scores/
```

## ✅ Validation de l'étape 9
- [ ] `python manage.py migrate` fonctionne
- [ ] POST /api/scores/ crée un score
- [ ] GET /api/scores/ retourne le top 10 en JSON
- [ ] Les scores sont triés par ordre décroissant

---

---

# ÉTAPE 10 — HttpClient C# → POST Score

## 🎯 Objectif
Depuis MonoGame, envoyer le score à l'API Django après une partie. Comprendre `async/await` et `HttpClient`.

## 📚 Théorie

### Async/Await dans MonoGame

MonoGame est synchrone (Update/Draw bloquants). Pour les appels réseau, on utilise `async/await` sans bloquer le thread principal :

```csharp
// ❌ Bloque le jeu pendant l'envoi
void PostScore() { _client.PostAsync(...).Wait(); }

// ✅ Non bloquant — le jeu continue pendant l'envoi
async Task PostScoreAsync() { await _client.PostAsync(...); }

// Appel depuis du code synchrone (Update) :
// On lance la tâche sans attendre le résultat
_ = PostScoreAsync(); // le _ ignore le résultat de la Task
```

### `System.Text.Json` — sérialisation JSON
```csharp
// Sérialiser un objet en JSON
var data = new { pseudo = "Kiza", points = 4200, wave = 5 };
string json = JsonSerializer.Serialize(data);
// → {"pseudo":"Kiza","points":4200,"wave":5}

// Désérialiser du JSON en liste
var scores = JsonSerializer.Deserialize<List<ScoreEntry>>(jsonString);
```

## 💻 Code

### `Managers/ApiManager.cs`
```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpaceInvaders.Managers;

// Structure pour désérialiser les scores reçus
public record ScoreEntry(
    int    Id,
    string Pseudo,
    int    Points,
    int    Wave,
    string Date
);

public class ApiManager
{
    private readonly HttpClient _client;
    private const string BASE_URL = "http://localhost:8000/api/";

    // État de la connexion
    public bool IsConnected  { get; private set; } = false;
    public bool IsLoading    { get; private set; } = false;
    public string LastError  { get; private set; } = "";

    public ApiManager()
    {
        _client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(5) // timeout de 5 secondes
        };
    }

    // Soumettre un score — non bloquant
    public async Task PostScoreAsync(string pseudo, int points, int wave)
    {
        IsLoading = true;
        try
        {
            var data    = new { pseudo, points, wave };
            var json    = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(BASE_URL + "scores/", content);

            IsConnected = response.IsSuccessStatusCode;
            LastError   = IsConnected ? "" : $"Erreur HTTP {(int)response.StatusCode}";
        }
        catch (Exception ex)
        {
            IsConnected = false;
            LastError   = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    // Récupérer le top 10
    public async Task<List<ScoreEntry>> GetLeaderboardAsync()
    {
        IsLoading = true;
        try
        {
            string json = await _client.GetStringAsync(BASE_URL + "scores/");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var scores  = JsonSerializer.Deserialize<List<ScoreEntry>>(json, options);

            IsConnected = true;
            return scores ?? new List<ScoreEntry>();
        }
        catch (Exception ex)
        {
            IsConnected = false;
            LastError   = ex.Message;
            return new List<ScoreEntry>();
        }
        finally
        {
            IsLoading = false;
        }
    }
}
```

### Appel depuis `GameOverScreen.cs`

```csharp
// Dans GameOverScreen — quand on soumet le score :
private ApiManager _api;
private bool _scoreSubmitted = false;

public async void SubmitScore(string pseudo, int points, int wave)
{
    await _api.PostScoreAsync(pseudo, points, wave);
    _scoreSubmitted = true;
}
```

## ✅ Validation de l'étape 10
- [ ] Après une partie, le score est envoyé à Django
- [ ] Le jeu ne freeze pas pendant l'envoi
- [ ] Si le serveur est hors ligne, le jeu continue normalement
- [ ] `LastError` contient le message d'erreur si ça échoue

---

---

# ÉTAPE 11 — Leaderboard In-Game

## 🎯 Objectif
Récupérer le top 10 depuis l'API et l'afficher dans un écran dédié du jeu.

## 💻 Code

### `Screens/LeaderboardScreen.cs`
```csharp
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SpaceInvaders.Managers;

namespace SpaceInvaders.Screens;

public class LeaderboardScreen : IScreen
{
    private SpriteBatch _spriteBatch;
    private FontManager _fontManager;
    private ApiManager  _apiManager;
    private Texture2D   _pixel;

    private List<ScoreEntry> _scores = new List<ScoreEntry>();
    private bool _loaded = false;

    private KeyboardState _previousKb;

    public LeaderboardScreen(SpriteBatch sb, FontManager fm, ApiManager api, Texture2D pixel)
    {
        _spriteBatch = sb;
        _fontManager = fm;
        _apiManager  = api;
        _pixel       = pixel;

        // Charger le leaderboard au démarrage de l'écran
        LoadLeaderboard();
    }

    private async void LoadLeaderboard()
    {
        _scores = await _apiManager.GetLeaderboardAsync();
        _loaded = true;
    }

    public void Update(GameTime gameTime)
    {
        KeyboardState kb = Keyboard.GetState();

        // Retour au menu avec Espace ou Échap
        if (kb.IsKeyDown(Keys.Space) && _previousKb.IsKeyUp(Keys.Space))
        {
            // Signaler à Game1 de changer d'écran
            OnReturnToMenu?.Invoke();
        }

        _previousKb = kb;
    }

    public event Action? OnReturnToMenu;

    public void Draw()
    {
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

        // Titre
        _spriteBatch.DrawString(_fontManager.Get("Title"), "🏆 TOP 10",
            new Vector2(300, 30), Color.Yellow);

        if (!_loaded || _apiManager.IsLoading)
        {
            _spriteBatch.DrawString(_fontManager.Get("UI"), "Chargement...",
                new Vector2(320, 300), Color.Gray);
        }
        else if (!_apiManager.IsConnected)
        {
            _spriteBatch.DrawString(_fontManager.Get("UI"), "Serveur non disponible",
                new Vector2(250, 280), Color.Red);
            _spriteBatch.DrawString(_fontManager.Get("UI"), _apiManager.LastError,
                new Vector2(200, 310), Color.Red * 0.7f);
        }
        else
        {
            // Afficher le top 10
            for (int i = 0; i < _scores.Count; i++)
            {
                var score = _scores[i];
                Color color = i == 0 ? Color.Gold :
                              i == 1 ? Color.Silver :
                              i == 2 ? Color.SandyBrown : Color.White;

                string line = $"{i + 1,2}. {score.Pseudo,-15} {score.Points,6} pts  vague {score.Wave}";
                _spriteBatch.DrawString(_fontManager.Get("UI"), line,
                    new Vector2(150, 120 + i * 40), color);
            }
        }

        // Instructions
        _spriteBatch.DrawString(_fontManager.Get("UI"), "ESPACE pour retourner au menu",
            new Vector2(240, 560), Color.Gray);

        _spriteBatch.End();
    }
}
```

## ✅ Validation de l'étape 11
- [ ] L'écran leaderboard s'affiche après une partie
- [ ] Le top 10 est chargé depuis l'API
- [ ] Si le serveur est hors ligne, un message d'erreur s'affiche
- [ ] Espace retourne au menu

---

---

# ÉTAPE 12 — Saisie du Pseudo

## 🎯 Objectif
Permettre au joueur de taper son pseudo avant de soumettre son score. Gérer la saisie clavier caractère par caractère.

## 📚 Théorie

### Saisie de texte dans MonoGame
MonoGame n'a pas de champ texte natif. Il faut détecter les touches et construire la chaîne manuellement :

```csharp
// Touches alphabétiques
Keys[] letters = { Keys.A, Keys.B, Keys.C, ... };

foreach (Keys key in letters)
{
    if (currentKb.IsKeyDown(key) && previousKb.IsKeyUp(key))
    {
        bool shift = currentKb.IsKeyDown(Keys.LeftShift);
        char c = shift ? key.ToString()[0] : char.ToLower(key.ToString()[0]);
        _pseudo += c;
    }
}

// Supprimer dernier caractère
if (currentKb.IsKeyDown(Keys.Back) && previousKb.IsKeyUp(Keys.Back))
    if (_pseudo.Length > 0)
        _pseudo = _pseudo[..^1]; // range operator C# 8+

// Valider
if (currentKb.IsKeyDown(Keys.Enter) && previousKb.IsKeyUp(Keys.Enter))
    if (_pseudo.Length > 0)
        SubmitAndGoToLeaderboard();
```

## ✅ Validation de l'étape 12
- [ ] Le joueur peut taper son pseudo (max 15 caractères)
- [ ] Backspace efface le dernier caractère
- [ ] Entrée soumet le score et va au leaderboard
- [ ] Le curseur clignote (cosmétique)

---

---

# ÉTAPES 13 & 14 — Finitions

## Étape 13 — Écran Leaderboard Animé
- Animation d'apparition des scores (slide-in)
- Surbrillance de ton propre score
- Effet de particules dorées pour le 1er

## Étape 14 — Shaders
Réutiliser ce qu'on sait du Pong :
- Glow sur le vaisseau et les balles
- Scanlines CRT pour l'ambiance rétro
- Effet de distorsion sur la mort du joueur (optionnel)

---

---

# 📖 Annexes

## Stack complète

```
Client MonoGame        Serveur Django
──────────────────     ──────────────────────────
Game1.cs               manage.py
Screens/               spaceinvaders/
  GameScreen             models.py (Score)
  LeaderboardScreen      serializers.py
  MenuScreen             views.py
  GameOverScreen         urls.py
  EnterNameScreen      requirements.txt
Managers/                django
  ApiManager             djangorestframework
  ScoreManager           psycopg2
  EnemyManager         settings.py
  BulletManager          CORS configuré
  SoundManager         PostgreSQL
  FontManager
  ParticleManager
Entities/
  Player, Enemy
  Bullet, Shield
```

## Commandes utiles

```bash
# Créer le projet MonoGame
dotnet new mgdesktopgl -n SpaceInvaders

# Créer le projet Django
django-admin startproject backend
cd backend
python manage.py startapp scores
pip install djangorestframework django-cors-headers

# Lancer les deux en parallèle
# Terminal 1 :
cd SpaceInvaders && dotnet run
# Terminal 2 :
cd backend && python manage.py runserver
```

## Cheatsheet HttpClient C#

```csharp
// GET
string json = await _client.GetStringAsync("http://...");

// POST JSON
var content = new StringContent(
    JsonSerializer.Serialize(data),
    Encoding.UTF8,
    "application/json"
);
var response = await _client.PostAsync("http://...", content);

// Désérialiser
var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
var result  = JsonSerializer.Deserialize<List<MonType>>(json, options);
```

## Pièges classiques à éviter

| Piège | Solution |
|-------|----------|
| `HttpClient` recréé à chaque appel | Instancier une seule fois au niveau classe |
| Appel API bloquant → freeze du jeu | Toujours utiliser `async/await` |
| Serveur hors ligne → crash | Toujours entourer d'un `try/catch` |
| Ennemis qui sortent de l'écran | Calculer `leftmost` et `rightmost` avant de bouger |
| Balle joueur traverse un ennemi | Vérifier la collision après `Update()` |
| Magic numbers partout | Tout mettre dans `GameSettings` |
| `new Random()` dans une boucle | Déclarer au niveau classe |

---

*Document créé — Projet Space Invaders MonoGame C# + Django API*
*À mettre à jour au fil de l'avancement.*
