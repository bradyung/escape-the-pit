using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GradientClimber
{
    public class TerrainMap
    {
        public int GridCols { get; }
        public int GridRows { get; }
        public int CellSize { get; }
        public double WorldMin { get; }
        public double WorldMax { get; }

        public double PeakX { get; private set; }
        public double PeakY { get; private set; }
        public double PeakHeight { get; private set; }

        public Level CurrentLevel { get; private set; }

        private double[,] _sampleHeights;
        private double _minHeight;
        private double _maxHeight;

        public TerrainMap(int gridCols, int gridRows, int cellSize, double worldMin, double worldMax, Level level)
        {
            GridCols = gridCols;
            GridRows = gridRows;
            CellSize = cellSize;
            WorldMin = worldMin;
            WorldMax = worldMax;
            CurrentLevel = level;

            _sampleHeights = new double[GridRows, GridCols];
            BuildSamples();
        }

        public void SetLevel(Level level)
        {
            CurrentLevel = level;
            BuildSamples();
        }

        public double Height(double x, double y)
        {
            return CurrentLevel.HeightFunc(x, y);
        }

        public double PartialX(double x, double y)
        {
            return CurrentLevel.FxFunc(x, y);
        }

        public double PartialY(double x, double y)
        {
            return CurrentLevel.FyFunc(x, y);
        }

        public PointF WorldToScreen(double x, double y)
        {
            float sx = (float)(((x - WorldMin) / (WorldMax - WorldMin)) * (GridCols * CellSize));
            float sy = (float)(((WorldMax - y) / (WorldMax - WorldMin)) * (GridRows * CellSize));
            return new PointF(sx, sy);
        }

        public Bitmap BuildBitmap()
        {
            Bitmap bmp = new Bitmap(GridCols * CellSize, GridRows * CellSize);

            using Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.None;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half;

            for (int row = 0; row < GridRows; row++)
            {
                for (int col = 0; col < GridCols; col++)
                {
                    double h = _sampleHeights[row, col];
                    double t = NormalizeHeight(h);

                    Color baseColor = HeightToColor(t);
                    Color shadedColor = ApplySimpleShading(row, col, baseColor);

                    using SolidBrush brush = new SolidBrush(shadedColor);
                    g.FillRectangle(brush, col * CellSize, row * CellSize, CellSize, CellSize);
                }
            }

            DrawContours(g);

            return bmp;
        }

        private void DrawContours(Graphics g)
        {
            using Pen majorContourPen = new Pen(Color.FromArgb(65, 0, 0, 0), 1);
            using Pen minorContourPen = new Pen(Color.FromArgb(28, 0, 0, 0), 1);

            for (int row = 0; row < GridRows - 1; row++)
            {
                for (int col = 0; col < GridCols - 1; col++)
                {
                    int q = Quantize(_sampleHeights[row, col]);
                    int qRight = Quantize(_sampleHeights[row, col + 1]);
                    int qDown = Quantize(_sampleHeights[row + 1, col]);

                    int x = col * CellSize;
                    int y = row * CellSize;

                    bool majorRight = IsMajorContour(q) || IsMajorContour(qRight);
                    bool majorDown = IsMajorContour(q) || IsMajorContour(qDown);

                    if (q != qRight)
                    {
                        g.DrawLine(
                            majorRight ? majorContourPen : minorContourPen,
                            x + CellSize - 1, y,
                            x + CellSize - 1, y + CellSize);
                    }

                    if (q != qDown)
                    {
                        g.DrawLine(
                            majorDown ? majorContourPen : minorContourPen,
                            x, y + CellSize - 1,
                            x + CellSize, y + CellSize - 1);
                    }
                }
            }
        }

        private void BuildSamples()
        {
            _minHeight = double.MaxValue;
            _maxHeight = double.MinValue;
            PeakHeight = double.MinValue;

            for (int row = 0; row < GridRows; row++)
            {
                for (int col = 0; col < GridCols; col++)
                {
                    double x = WorldMin + (WorldMax - WorldMin) * col / (GridCols - 1.0);
                    double y = WorldMin + (WorldMax - WorldMin) * row / (GridRows - 1.0);

                    double h = Height(x, y);
                    _sampleHeights[row, col] = h;

                    if (h < _minHeight) _minHeight = h;
                    if (h > _maxHeight) _maxHeight = h;

                    if (h > PeakHeight)
                    {
                        PeakHeight = h;
                        PeakX = x;
                        PeakY = y;
                    }
                }
            }
        }

        private double NormalizeHeight(double h)
        {
            if (Math.Abs(_maxHeight - _minHeight) < 0.000001)
                return 0.5;

            return (h - _minHeight) / (_maxHeight - _minHeight);
        }

        private int Quantize(double h)
        {
            double t = NormalizeHeight(h);
            return (int)(t * 10);
        }

        private bool IsMajorContour(int q)
        {
            return q % 2 == 0;
        }

        private Color ApplySimpleShading(int row, int col, Color baseColor)
        {
            int left = Math.Max(0, col - 1);
            int right = Math.Min(GridCols - 1, col + 1);
            int up = Math.Max(0, row - 1);
            int down = Math.Min(GridRows - 1, row + 1);

            double dx = _sampleHeights[row, right] - _sampleHeights[row, left];
            double dy = _sampleHeights[down, col] - _sampleHeights[up, col];

            double shade = (-dx * 0.6) + (-dy * 0.4);
            shade = Math.Max(-0.18, Math.Min(0.18, shade));

            return AdjustBrightness(baseColor, shade);
        }

        private Color AdjustBrightness(Color color, double amount)
        {
            int r = ClampToByte(color.R + (int)(255 * amount));
            int g = ClampToByte(color.G + (int)(255 * amount));
            int b = ClampToByte(color.B + (int)(255 * amount));

            return Color.FromArgb(r, g, b);
        }

        private int ClampToByte(int value)
        {
            if (value < 0) return 0;
            if (value > 255) return 255;
            return value;
        }

        private Color HeightToColor(double t)
        {
            if (t < 0.12) return Color.FromArgb(18, 38, 92);
            if (t < 0.24) return Color.FromArgb(38, 86, 140);
            if (t < 0.38) return Color.FromArgb(50, 112, 82);
            if (t < 0.52) return Color.FromArgb(78, 128, 68);
            if (t < 0.66) return Color.FromArgb(156, 128, 58);
            if (t < 0.80) return Color.FromArgb(188, 122, 54);
            if (t < 0.92) return Color.FromArgb(170, 102, 102);
            return Color.FromArgb(225, 225, 225);
        }
    }
}