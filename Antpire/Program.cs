namespace Antpire;
public static class Program {
    [STAThread]
    static void Main() {
        using var game = new Antpire();
        game.Run();
    }
}

