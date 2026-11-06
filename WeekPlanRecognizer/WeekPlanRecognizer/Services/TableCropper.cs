
using OpenCvSharp;

namespace WeekPlanRecognizer.Services;

public class TableCropper
{
    public Mat CropTable(Mat input)
    {
        // Graustufen
        using var gray = new Mat();
        Cv2.CvtColor(
            input,
            gray,
            ColorConversionCodes.BGR2GRAY
        );

        // Dunkle Linien hervorheben
        using var binary = new Mat();
        Cv2.Threshold(
            gray,
            binary,
            160,
            255,
            ThresholdTypes.BinaryInv
        );

        // Horizontale und vertikale Linien isolieren
        using var horizontal = new Mat();
        using var vertical = new Mat();

        using var hKernel = Cv2.GetStructuringElement(
            MorphShapes.Rect,
            new Size(Math.Max(20, input.Width / 12), 1)
        );

        using var vKernel = Cv2.GetStructuringElement(
            MorphShapes.Rect,
            new Size(1, Math.Max(20, input.Height / 12))
        );

        Cv2.MorphologyEx(
            binary, horizontal,
            MorphTypes.Open, hKernel
        );

        Cv2.MorphologyEx(
            binary, vertical,
            MorphTypes.Open, vKernel
        );

        Cv2.ImWrite("debug_horizontal_v2.png", horizontal);
        Cv2.ImWrite("debug_vertical_v2.png", vertical);

        // Projektion: Anzahl heller Pixel je Zeile/Spalte
        int[] rowScores = new int[input.Height];
        int[] colScores = new int[input.Width];

        for (int y = 0; y < input.Height; y++)
        {
            using var row = horizontal.Row(y);
            rowScores[y] = Cv2.CountNonZero(row);
        }

        for (int x = 0; x < input.Width; x++)
        {
            using var col = vertical.Col(x);
            colScores[x] = Cv2.CountNonZero(col);
        }

        // Linienkandidaten
        int rowThreshold = (int)(input.Width * 0.35);
        int colThreshold = (int)(input.Height * 0.25);

        var rows = Enumerable.Range(0, input.Height)
            .Where(y => rowScores[y] > rowThreshold)
            .ToArray();

        var cols = Enumerable.Range(0, input.Width)
            .Where(x => colScores[x] > colThreshold)
            .ToArray();

        if (rows.Length < 2 || cols.Length < 2)
        {
            throw new InvalidOperationException(
                "Nicht genügend Tabellenlinien erkannt."
            );
        }

        int left = cols.Min();
        int right = cols.Max();
        int top = rows.Min();
        int bottom = rows.Max();

        // Sicherheitsabstand für Überschriften
        int paddingX = 25;
        int paddingY = 45;

        left = Math.Max(0, left - paddingX);
        right = Math.Min(input.Width - 1, right + paddingX);
        top = Math.Max(0, top - paddingY);
        bottom = Math.Min(input.Height - 1, bottom + paddingY);

        var rect = new Rect(
            left,
            top,
            right - left + 1,
            bottom - top + 1
        );

        using var debug = input.Clone();

        Cv2.Rectangle(
            debug,
            rect,
            new Scalar(0, 0, 255),
            3
        );

        Cv2.ImWrite("debug_table_bounds_v2.png", debug);

        return new Mat(input, rect).Clone();
    }
}
