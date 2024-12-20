// See https://aka.ms/new-console-template for more information

using SemiAutomaticController;

class TestClass
{
    private static int[] positions= new int[6];
    private static int[] speeds= new int[6];
    private static bool movementDirection = true; //true = ++ false = --

    static void Main(string[] args)
    {
        for (int i = 0; i < 6; i++)
        {
            positions[i] = 0; 
            speeds[i] = 0; 
        }


        TCPServer server = new TCPServer("192.168.137.123", 32760);        
        server.StartListening();
        
        Console.WriteLine("Enter the refresh rate (calls per second):");
        Double.TryParse(Console.ReadLine(), out double refreshRate);
        Console.WriteLine("Enter the minimum output value:");
        Int32.TryParse(Console.ReadLine(), out int minDistance);
        Console.WriteLine("Enter the maximum output value:");
        Int32.TryParse(Console.ReadLine(), out int maxDistance);
        Console.WriteLine("Enter the velocity:");
        Int32.TryParse(Console.ReadLine(), out int velocity);
        Console.WriteLine("Enter the increment:");
        Int32.TryParse(Console.ReadLine(), out int increment);
        
        for (int i = 0; i < speeds.Count(); i++)
            speeds[i] = velocity;

        for (int i = 0; i < positions.Count(); i++)
            positions[i] = minDistance;


        new Thread(() =>
        {
            while (true)
            {
                Update(increment, minDistance, maxDistance, server);
                Thread.Sleep((int)((1.0f / refreshRate) * 1000));
            }
        }).Start();
    }

    private static void Update(int increment, int minD, int maxD, TCPServer server)
    {
        if (movementDirection)
        {
            if (positions[0] + increment > maxD)
                movementDirection = !movementDirection;
        }
        else
        {
            if (positions[0] - increment < minD)
                movementDirection = !movementDirection;
        }

        if (movementDirection)
            for (int i = 0; i < positions.Count(); i++)
                positions[i] += increment;
        else
            for (int i = 0; i < positions.Count(); i++)
                positions[i] -= increment;


        server.SendData(Parser.Parse(positions, speeds));
    }
}