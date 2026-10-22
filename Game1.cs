using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Ctverecek;

public sealed class Game1 : Game
{
    private const int WindowWidth = 800;
    private const int WindowHeight = 480;
    private const int PlayerSize = 32;
    private const int TargetSize = 24;
    private const float PlayerSpeed = 240f;

    private readonly GraphicsDeviceManager _graphics;
    private readonly Random _random = new();
    private SpriteBatch _spriteBatch = null!;
    private Texture2D _pixel = null!;
    private Vector2 _playerPosition = new(80, 80);
    private Rectangle _target;
    private int _score;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = WindowWidth,
            PreferredBackBufferHeight = WindowHeight
        };

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.Title = "Ctverecek - WASD nebo sipky: sbirej zlute ctverce";
    }

    protected override void Initialize()
    {
        SpawnTarget();
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
        KeyboardState keyboard = Keyboard.GetState();
        if (keyboard.IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        Vector2 direction = Vector2.Zero;
        if (keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.A))
        {
            direction.X -= 1;
        }
        if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D))
        {
            direction.X += 1;
        }
        if (keyboard.IsKeyDown(Keys.Up) || keyboard.IsKeyDown(Keys.W))
        {
            direction.Y -= 1;
        }
        if (keyboard.IsKeyDown(Keys.Down) || keyboard.IsKeyDown(Keys.S))
        {
            direction.Y += 1;
        }

        if (direction != Vector2.Zero)
        {
            direction.Normalize();
            _playerPosition += direction * PlayerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        _playerPosition.X = MathHelper.Clamp(_playerPosition.X, 0, WindowWidth - PlayerSize);
        _playerPosition.Y = MathHelper.Clamp(_playerPosition.Y, 0, WindowHeight - PlayerSize);

        Rectangle player = PlayerRectangle;
        if (player.Intersects(_target))
        {
            _score++;
            Window.Title = $"Ctverecek - Skore: {_score} | WASD nebo sipky";
            SpawnTarget();
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(28, 34, 48));

        _spriteBatch.Begin();
        _spriteBatch.Draw(_pixel, PlayerRectangle, new Color(80, 170, 255));
        _spriteBatch.Draw(_pixel, _target, Color.Gold);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private Rectangle PlayerRectangle => new(
        (int)_playerPosition.X,
        (int)_playerPosition.Y,
        PlayerSize,
        PlayerSize);

    private void SpawnTarget()
    {
        do
        {
            _target = new Rectangle(
                _random.Next(0, WindowWidth - TargetSize + 1),
                _random.Next(0, WindowHeight - TargetSize + 1),
                TargetSize,
                TargetSize);
        }
        while (_target.Intersects(PlayerRectangle));
    }
}
