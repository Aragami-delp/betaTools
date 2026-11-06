using OpenCvSharp;
using WeekPlanRecognizer.Services;

namespace WeekPlanRecognizer;

class Program
{
    private const string InputPath = "wochenplan.jpg";

    static void Main(string[] args)
    {
        using var original = Cv2.ImRead(InputPath);

        if (original.Empty())
        {
            Console.WriteLine("Bild nicht gefunden.");
            return;
        }

        using var rotated = new Mat();

        Cv2.Rotate(
            original,
            rotated,
            RotateFlags.Rotate90Counterclockwise
        );

        var detector = new GridLineDetector();

        using var result = detector.DetectLines(rotated);

        Console.WriteLine("Linienerkennung abgeschlossen.");

    }
}