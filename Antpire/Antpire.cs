using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;


namespace Antpire;
public class Antpire : Game {
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private readonly ScreenManager screenManager;

    public Antpire() {
        Window.AllowUserResizing = true;
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        screenManager = new ScreenManager();
        Components.Add(screenManager);
    }

    protected override void Initialize() {
        base.Initialize();
        loadLogoScreen();
    }

    protected override void LoadContent() {
        spriteBatch = new SpriteBatch(GraphicsDevice);

    }

    protected override void Update(GameTime gameTime) {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        screenManager.Update(gameTime);


        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.Black);

        screenManager.Draw(gameTime);

        base.Draw(gameTime);
    }

    private void loadLogoScreen() {
        screenManager.LoadScreen(new Screens.SimulationScreen(this), new FadeTransition(GraphicsDevice, Color.White));
    }
}
