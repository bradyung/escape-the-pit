using System.Drawing;

namespace GradientClimber
{
    public static class HudRenderer
    {
        public static void Draw(
            Graphics g,
            int mapWidth,
            int hudWidth,
            int clientHeight,
            string levelName,
            string difficulty,
            string mode,
            int score,
            int timeLeft,
            double playerX,
            double playerY,
            double height,
            double gradientMagnitude,
            string hintsLeft,
            string stepsLeft,
            string goalText,
            string message)
        {
            using SolidBrush bg = new SolidBrush(Color.FromArgb(238, 22, 22, 22));
            using Font titleFont = new Font("Segoe UI", 14, FontStyle.Bold);
            using Font bodyFont = new Font("Segoe UI", 10, FontStyle.Regular);
            using Font smallFont = new Font("Segoe UI", 9, FontStyle.Regular);

            g.FillRectangle(bg, mapWidth, 0, hudWidth, clientHeight);

            int x = mapWidth + 14;
            int y = 16;

            g.DrawString(levelName, titleFont, Brushes.Gold, x, y);
            y += 34;

            g.DrawString($"Score: {score}", bodyFont, Brushes.White, x, y);
            y += 20;
            g.DrawString($"Time: {timeLeft}s", bodyFont, Brushes.White, x, y);
            y += 20;
            g.DrawString($"Difficulty: {difficulty}", bodyFont, Brushes.White, x, y);
            y += 20;
            g.DrawString($"Mode: {mode}", bodyFont, Brushes.White, x, y);
            y += 30;

            g.DrawString("Math", titleFont, Brushes.Gold, x, y);
            y += 28;
            g.DrawString($"Pos: ({playerX:F1}, {playerY:F1})", bodyFont, Brushes.White, x, y);
            y += 20;
            g.DrawString($"f(x,y): {height:F2}", bodyFont, Brushes.White, x, y);
            y += 20;
            g.DrawString($"|∇f|: {gradientMagnitude:F2}", bodyFont, Brushes.White, x, y);
            y += 30;

            g.DrawString("Resources", titleFont, Brushes.Gold, x, y);
            y += 28;
            g.DrawString($"Hints: {hintsLeft}", bodyFont, Brushes.White, x, y);
            y += 20;
            g.DrawString($"Steps: {stepsLeft}", bodyFont, Brushes.White, x, y);
            y += 30;

            g.DrawString("Goal", titleFont, Brushes.Gold, x, y);
            y += 28;
            g.DrawString(goalText, smallFont, Brushes.White, new RectangleF(x, y, hudWidth - 28, 40));

            if (!string.IsNullOrWhiteSpace(message))
            {
                using SolidBrush msgBg = new SolidBrush(Color.FromArgb(170, 0, 0, 0));
                Rectangle rect = new Rectangle(mapWidth + 10, clientHeight - 95, hudWidth - 20, 72);
                g.FillRectangle(msgBg, rect);
                g.DrawRectangle(Pens.Gold, rect);
                g.DrawString(message, bodyFont, Brushes.White, new RectangleF(rect.X + 10, rect.Y + 10, rect.Width - 20, rect.Height - 20));
            }
        }
    }
}