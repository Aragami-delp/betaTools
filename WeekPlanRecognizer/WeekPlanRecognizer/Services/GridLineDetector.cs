using OpenCvSharp;

namespace WeekPlanRecognizer.Services;

public class GridLineDetector
{
    public Mat DetectLines(Mat input)
    {
        using var gray = new Mat();
        Cv2.CvtColor(
            input,
            gray,
            ColorConversionCodes.BGR2GRAY
        );

        // Lokalen Kontrast verbessern
        using var clahe = Cv2.CreateCLAHE(
            clipLimit: 2.0,
            tileGridSize: new Size(8, 8)
        );

        using var enhanced = new Mat();
        clahe.Apply(gray, enhanced);

        Cv2.ImWrite("debug_enhanced.png", enhanced);

        // Kanten erkennen
        using var blurred = new Mat();
        Cv2.GaussianBlur(
            enhanced,
            blurred,
            new Size(3, 3),
            0
        );

        using var edges = new Mat();
        Cv2.Canny(
            blurred,
            edges,
            50,
            150
        );

        Cv2.ImWrite("debug_edges.png", edges);

        // Liniensegmente erkennen
        LineSegmentPoint[] lines = Cv2.HoughLinesP(
            edges,
            rho: 1,
            theta: Math.PI / 180,
            threshold: 60,
            minLineLength: 80,
            maxLineGap: 25
        );

        var result = input.Clone();

        foreach (var line in lines)
        {
            double angle = Math.Atan2(
                line.P2.Y - line.P1.Y,
                line.P2.X - line.P1.X
            ) * 180 / Math.PI;

            // Horizontale Linien grün
            if (Math.Abs(angle) < 10 ||
                Math.Abs(angle) > 170)
            {
                Cv2.Line(
                    result,
                    line.P1,
                    line.P2,
                    new Scalar(0, 255, 0),
                    2
                );
            }
            // Vertikale Linien rot
            else if (Math.Abs(Math.Abs(angle) - 90) < 10)
            {
                Cv2.Line(
                    result,
                    line.P1,
                    line.P2,
                    new Scalar(0, 0, 255),
                    2
                );
            }
        }

        Cv2.ImWrite("debug_hough_lines.png", result);

        Console.WriteLine(
            $"Erkannte Liniensegmente: {lines.Length}"
        );

        return result;
    }
}
