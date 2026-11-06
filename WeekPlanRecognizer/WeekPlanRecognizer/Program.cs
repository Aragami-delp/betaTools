using WeekPlanRecognizer.Services;
using OpenCvSharp;

namespace WeekPlanRecognizer;

class Program
{
    private const string InputPath = "Wochenplan_Prepared.jpg";
    
    static void Main(string[] args)
    {
        using var image = Cv2.ImRead(InputPath);

        using var gray = new Mat();
        Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);

        using var binary = new Mat();

        Cv2.AdaptiveThreshold(
            gray,
            binary,
            255,
            AdaptiveThresholdTypes.GaussianC,
            ThresholdTypes.BinaryInv,
            31,
            10
        );

// Horizontale Linien hervorheben
        using var horizontal = new Mat();

        using var horizontalKernel =
            Cv2.GetStructuringElement(
                MorphShapes.Rect,
                new Size(30, 1)
            );

        Cv2.MorphologyEx(
            binary,
            horizontal,
            MorphTypes.Open,
            horizontalKernel
        );

// Vertikale Linien hervorheben
        using var vertical = new Mat();

        using var verticalKernel =
            Cv2.GetStructuringElement(
                MorphShapes.Rect,
                new Size(1, 30)
            );

        Cv2.MorphologyEx(
            binary,
            vertical,
            MorphTypes.Open,
            verticalKernel
        );

// Beide Linienbilder kombinieren
        using var lines = new Mat();

        Cv2.BitwiseOr(horizontal, vertical, lines);

        Cv2.ImWrite("lines.png", lines);
    }
    
    
}