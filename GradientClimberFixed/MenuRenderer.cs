using System.Drawing;

namespace GradientClimber
{
    public static class MenuRenderer
    {
        public static void DrawTitleScreen(Graphics g, Size clientSize)
        {
            g.Clear(Color.FromArgb(18, 18, 28));

            using Font title = new Font("Segoe UI", 28, FontStyle.Bold);
            using Font subtitle = new Font("Segoe UI", 13, FontStyle.Regular);
            using Font body = new Font("Segoe UI", 11, FontStyle.Regular);

            g.DrawString("Gradient Climber", title, Brushes.Gold, 215, 90);
            g.DrawString("A multivariable calculus climbing game", subtitle, Brushes.White, 205, 150);

            string text =
                "Move uphill by reading the terrain and using the gradient.\n\n" +
                "Modes:\n" +
                "1. Classic\n" +
                "2. Endless\n\n" +
                "Press Enter to continue";

            g.DrawString(text, body, Brushes.WhiteSmoke, new RectangleF(220, 225, 420, 200));
        }

        
        public static void DrawDifficultyScreen(Graphics g, Size clientSize, SaveData saveData)
        {
            g.Clear(Color.FromArgb(18, 18, 28));

            using Font title = new Font("Segoe UI", 22, FontStyle.Bold);
            using Font body = new Font("Segoe UI", 11, FontStyle.Regular);
            using Font small = new Font("Segoe UI", 10, FontStyle.Regular);

            g.DrawString("Choose Difficulty", title, Brushes.Gold, 90, 28);

            string hardText = saveData.HardUnlocked ? "" : " (Locked)";
            string expertText = saveData.ExpertUnlocked ? "" : " (Locked)";

            int x = 40;
            int y = 90;
            int gap = 64;

            g.DrawString("1 - Easy", body, Brushes.White, x, y);
            g.DrawString("More time and more help", small, Brushes.WhiteSmoke, x, y + 28);

            y += gap;
            g.DrawString("2 - Normal", body, Brushes.White, x, y);
            g.DrawString("Balanced challenge", small, Brushes.WhiteSmoke, x, y + 28);

            y += gap;
            g.DrawString($"3 - Hard{hardText}", body, Brushes.White, x, y);
            g.DrawString("Less time and less help", small, Brushes.WhiteSmoke, x, y + 28);

            y += gap;
            g.DrawString($"4 - Expert{expertText}", body, Brushes.White, x, y);
            g.DrawString("Minimal help", small, Brushes.WhiteSmoke, x, y + 28);

            g.DrawString("Press 1-4", small, Brushes.Gold, x, clientSize.Height - 40);
        }

        public static void DrawModeScreen(Graphics g, Size clientSize, SaveData saveData)
        {
            g.Clear(Color.FromArgb(18, 18, 28));
        
            using Font title = new Font("Segoe UI", 22, FontStyle.Bold);
            using Font body = new Font("Segoe UI", 11, FontStyle.Regular);
            using Font small = new Font("Segoe UI", 10, FontStyle.Regular);
        
            g.DrawString("Choose Mode", title, Brushes.Gold, 85, 28);
        
            int x = 40;
            int y = 90;
            int gap = 82;
        
            g.DrawString("1 - Classic", body, Brushes.White, x, y);
            g.DrawString("Play the main levels", small, Brushes.WhiteSmoke, x, y + 28);
        
            y += gap;
            g.DrawString("2 - Endless", body, Brushes.White, x, y);
            g.DrawString("Random mountains forever", small, Brushes.WhiteSmoke, x, y + 28);
        
            y += 86;
            g.DrawString($"Best Classic: {saveData.BestScoreClassic}", small, Brushes.White, x, y);
            y += 22;
            g.DrawString($"Best Endless: {saveData.BestScoreEndless}", small, Brushes.White, x, y);
        
            g.DrawString("Press 1-2", small, Brushes.Gold, x, clientSize.Height - 40);
        }
        public static void DrawOverlay(Graphics g, Size clientSize, string title, string subtitle)
        {
            int width = 420;
            int height = 180;
            int x = (clientSize.Width - width) / 2;
            int y = (clientSize.Height - height) / 2;

            using SolidBrush bg = new SolidBrush(Color.FromArgb(185, 0, 0, 0));
            using Font titleFont = new Font("Segoe UI", 24, FontStyle.Bold);
            using Font bodyFont = new Font("Segoe UI", 12, FontStyle.Regular);

            g.FillRectangle(bg, x, y, width, height);
            g.DrawRectangle(Pens.Gold, x, y, width, height);
            g.DrawString(title, titleFont, Brushes.Gold, x + 105, y + 25);
            g.DrawString(subtitle, bodyFont, Brushes.White, new RectangleF(x + 70, y + 85, 280, 70));
        }
    }
}