namespace SemiAutomaticController;

public static class Parser
{
    public static string Parse(int[] positions, int[] speeds)
    {
        string tempSendingData = "{\"positions\" : [";

        tempSendingData += CheckDigitCount(positions[0]);

        for (int i = 1; i < positions.Count(); i++)
            tempSendingData += ", " + CheckDigitCount(positions[i]);

        tempSendingData += "], \"speeds\" : [";

        //Console.WriteLine(CheckDigitCount(speeds[0]));
        tempSendingData += CheckDigitCount(speeds[0]);

        for (int i = 1; i < speeds.Count(); i++)
        {
            //Console.WriteLine(CheckDigitCount(speeds[i]));
            tempSendingData += ", " + CheckDigitCount(speeds[i]);
        }

        tempSendingData += "]}";

        Console.WriteLine($"Parsed into: \"{tempSendingData}\"");
        return tempSendingData;
    }
    
    private static string CheckDigitCount(int value)
    {
        if (value >= 100)
            return value.ToString();
        else if (value < 100 && value >= 10)
            return "0" + value;
        else
            return "00" + value;
    }
}