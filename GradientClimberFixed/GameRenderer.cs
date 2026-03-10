using System;
using System.Collections.Generic;
using System.Drawing;

namespace GradientClimber
{
    public static class GameRenderer
    {
        public static void DrawWorld(
            Graphics g,
            TerrainMap terrain,
            Bitmap terrainBitmap,
            Player player,
            List<PointF> falseSummits,
            List<bool> falseSummitTriggered,
            List<PointF> winParticles,
            bool showPeakAfterWin,
            bool showSmallGradientArrow,
            bool showBigHintArrow,
            float pulseTime,
            int fogRadiusCells)
        {
            DrawVisibleTerrainOnly(g, terrain, terrainBitmap, player, fogRadiusCells);

            DrawTrail(g, terrain, player, fogRadiusCells);
            DrawFalseSummits(g, terrain, falseSummits, falseSummitTriggered, player, fogRadiusCells);

            if (showPeakAfterWin)
            {
                DrawPeak(g, terrain, pulseTime, player, fogRadiusCells);
            }

            DrawPlayer(g, terrain, player);

            if (showSmallGradientArrow)
            {
                DrawGradientArrow(g, terrain, player, 34, Color.White, 3);
            }

            if (showBigHintArrow)
            {
                DrawPeakHintArrow(g, terrain, player, 85, Color.Cyan, 5);
            }

            DrawWinParticles(g, player, winParticles, terrain, fogRadiusCells);
        }
        private static void DrawPeakHintArrow(Graphics g, TerrainMap terrain, Player player, float length, Color color, float thickness)
        {
            PointF p = terrain.WorldToScreen(player.X, player.Y);
        
            double dx = terrain.PeakX - player.X;
            double dy = terrain.PeakY - player.Y;
            double mag = Math.Sqrt(dx * dx + dy * dy);
        
            if (mag < 0.0001) return;
        
            dx /= mag;
            dy /= mag;
        
            float endX = p.X + (float)(dx * length);
            float endY = p.Y - (float)(dy * length);
        
            using Pen pen = new Pen(color, thickness);
            using SolidBrush brush = new SolidBrush(color);
        
            g.DrawLine(pen, p.X, p.Y, endX, endY);
            g.FillEllipse(brush, endX - 5, endY - 5, 10, 10);
        }

        private static void DrawVisibleTerrainOnly(Graphics g, TerrainMap terrain, Bitmap terrainBitmap, Player player, int fogRadiusCells)
        {
            int mapWidth = terrain.GridCols * terrain.CellSize;
            int mapHeight = terrain.GridRows * terrain.CellSize;

            g.FillRectangle(Brushes.Black, 0, 0, mapWidth, mapHeight);

            Point playerCell = WorldToCell(terrain, player.X, player.Y);

            for (int row = 0; row < terrain.GridRows; row++)
            {
                for (int col = 0; col < terrain.GridCols; col++)
                {
                    int dx = col - playerCell.X;
                    int dy = row - playerCell.Y;

                    bool visible = (dx * dx + dy * dy) <= fogRadiusCells * fogRadiusCells;
                    if (!visible)
                        continue;

                    int x = col * terrain.CellSize;
                    int y = row * terrain.CellSize;
                    Rectangle dest = new Rectangle(x, y, terrain.CellSize, terrain.CellSize);
                    Rectangle src = new Rectangle(x, y, terrain.CellSize, terrain.CellSize);

                    g.DrawImage(terrainBitmap, dest, src, GraphicsUnit.Pixel);
                }
            }
        }

        private static void DrawPeak(Graphics g, TerrainMap terrain, float pulseTime, Player player, int fogRadiusCells)
        {
            if (!IsWorldPointVisible(terrain, player, terrain.PeakX, terrain.PeakY, fogRadiusCells))
                return;

            PointF peak = terrain.WorldToScreen(terrain.PeakX, terrain.PeakY);
            float pulse = 20 + 6 * (float)Math.Sin(pulseTime * 2f);

            using Pen glow = new Pen(Color.Gold, 4);
            g.DrawEllipse(glow, peak.X - pulse / 2, peak.Y - pulse / 2, pulse, pulse);
            g.FillEllipse(Brushes.Gold, peak.X - 5, peak.Y - 5, 10, 10);
        }

        private static void DrawPlayer(Graphics g, TerrainMap terrain, Player player)
        {
            PointF p = terrain.WorldToScreen(player.X, player.Y);

            float glowRadius = 18 + 3 * (float)Math.Sin(player.GlowPhase);
            using SolidBrush glow = new SolidBrush(Color.FromArgb(70, 80, 220, 255));
            g.FillEllipse(glow, p.X - glowRadius / 2, p.Y - glowRadius / 2, glowRadius, glowRadius);

            g.FillEllipse(Brushes.Black, p.X - 8, p.Y - 8, 16, 16);
            g.DrawEllipse(Pens.White, p.X - 8, p.Y - 8, 16, 16);
        }

        private static void DrawTrail(Graphics g, TerrainMap terrain, Player player, int fogRadiusCells)
        {
            if (player.Trail.Count < 2) return;

            for (int i = 1; i < player.Trail.Count; i++)
            {
                PointF a = player.Trail[i - 1];
                PointF b = player.Trail[i];

                if (!IsScreenPointVisible(terrain, player, a, fogRadiusCells) &&
                    !IsScreenPointVisible(terrain, player, b, fogRadiusCells))
                {
                    continue;
                }

                int alpha = 40 + (int)(180.0 * i / player.Trail.Count);
                using Pen pen = new Pen(Color.FromArgb(alpha, 255, 255, 255), 2);
                g.DrawLine(pen, a, b);
            }
        }

        private static void DrawFalseSummits(Graphics g, TerrainMap terrain, List<PointF> falseSummits, List<bool> triggered, Player player, int fogRadiusCells)
        {
            using Pen pen = new Pen(Color.MediumPurple, 2);

            for (int i = 0; i < falseSummits.Count; i++)
            {
                if (!triggered[i]) continue;
                if (!IsWorldPointVisible(terrain, player, falseSummits[i].X, falseSummits[i].Y, fogRadiusCells))
                    continue;

                PointF p = terrain.WorldToScreen(falseSummits[i].X, falseSummits[i].Y);
                g.DrawEllipse(pen, p.X - 8, p.Y - 8, 16, 16);
                g.DrawLine(pen, p.X - 8, p.Y - 8, p.X + 8, p.Y + 8);
                g.DrawLine(pen, p.X + 8, p.Y - 8, p.X - 8, p.Y + 8);
            }
        }

        private static void DrawGradientArrow(Graphics g, TerrainMap terrain, Player player, float length, Color color, float thickness)
        {
            PointF p = terrain.WorldToScreen(player.X, player.Y);

            double gx = terrain.PartialX(player.X, player.Y);
            double gy = terrain.PartialY(player.X, player.Y);
            double mag = Math.Sqrt(gx * gx + gy * gy);

            if (mag < 0.0001) return;

            gx /= mag;
            gy /= mag;

            float endX = p.X + (float)(gx * length);
            float endY = p.Y - (float)(gy * length);

            using Pen pen = new Pen(color, thickness);
            using SolidBrush brush = new SolidBrush(color);

            g.DrawLine(pen, p.X, p.Y, endX, endY);
            g.FillEllipse(brush, endX - 4, endY - 4, 8, 8);
        }

        private static void DrawWinParticles(Graphics g, Player player, List<PointF> winParticles, TerrainMap terrain, int fogRadiusCells)
        {
            foreach (PointF p in winParticles)
            {
                if (!IsScreenPointVisible(terrain, player, p, fogRadiusCells))
                    continue;

                g.FillEllipse(Brushes.Gold, p.X - 2, p.Y - 2, 4, 4);
            }
        }

        private static bool IsWorldPointVisible(TerrainMap terrain, Player player, double worldX, double worldY, int fogRadiusCells)
        {
            Point playerCell = WorldToCell(terrain, player.X, player.Y);
            Point targetCell = WorldToCell(terrain, worldX, worldY);

            int dx = targetCell.X - playerCell.X;
            int dy = targetCell.Y - playerCell.Y;

            return (dx * dx + dy * dy) <= fogRadiusCells * fogRadiusCells;
        }

        private static bool IsScreenPointVisible(TerrainMap terrain, Player player, PointF screenPoint, int fogRadiusCells)
        {
            Point playerCell = WorldToCell(terrain, player.X, player.Y);

            int col = (int)(screenPoint.X / terrain.CellSize);
            int row = (int)(screenPoint.Y / terrain.CellSize);

            int dx = col - playerCell.X;
            int dy = row - playerCell.Y;

            return (dx * dx + dy * dy) <= fogRadiusCells * fogRadiusCells;
        }

        private static Point WorldToCell(TerrainMap terrain, double x, double y)
        {
            int col = (int)((x - terrain.WorldMin) / (terrain.WorldMax - terrain.WorldMin) * (terrain.GridCols - 1));
            int row = (int)((terrain.WorldMax - y) / (terrain.WorldMax - terrain.WorldMin) * (terrain.GridRows - 1));

            col = Math.Max(0, Math.Min(terrain.GridCols - 1, col));
            row = Math.Max(0, Math.Min(terrain.GridRows - 1, row));

            return new Point(col, row);
        }
    }
}