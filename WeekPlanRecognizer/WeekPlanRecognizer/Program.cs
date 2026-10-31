using OpenCvSharp;

namespace WeekPlanRecognizer;

class Program
{
    private const string inputPath = "wochenplan.jpg";
    private const string outputPath = "wochenplan_rotated.png";
    
    static void Main(string[] args)
    {
        using Mat original = Cv2.ImRead(inputPath);
        if (original.Empty())
        {
            Console.WriteLine("No image found");
            return;
        }

        using Mat rotated = new();
        
        Cv2.Rotate(original, rotated, RotateFlags.Rotate90Clockwise);
        
        Cv2.ImWrite(outputPath, rotated);

        Console.WriteLine($"Image saved: {outputPath}");
    }
}