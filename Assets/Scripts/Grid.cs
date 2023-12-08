public class Grid
{
    public int Width { get; set; }
    public int Height { get; set; }
    public Node[,] Nodes { get; set; }

    // Weitere Eigenschaften und Methoden der Grid-Klasse...

    public Grid(int width, int height)
    {
        Width = width;
        Height = height;
        Nodes = new Node[width, height];
    }
}
