using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing.Drawing2D;
using System.Drawing;

namespace NodeEditor
{
    [Flags]
    public enum RoundedSide : uint
    {
        None        = 0x0000,
        Top         = 0x0001,
        Bottom      = 0x0002,
    }

    [Flags]
    public enum SelectionMode : uint
    {
        None        = 0x0000,
        Hover       = 0x0001,
        Selected    = 0x0002,
        Warning     = 0x0004,
        Error       = 0x0008,
    }

    public class NodeFrame
    {
        public string Title { get; set; }

        public GraphicsPath BodyFrame { get; set; }

        public GraphicsPath TitleFrame { get; set; }

        private RectangleF hoverRect;

        // TODO List on input pins

        // TODO List of output pins


//        protected static Brush[] TitleBrush = { Brushes.Aquamarine, Brushes.Cyan, Brushes.Goldenrod, Brushes.Gold };
        protected static Brush[] TitleBrush = { new SolidBrush(Color.FromArgb(255, 85, 110, 115)), new SolidBrush(Color.FromArgb(255, 105, 130, 135)), new SolidBrush(Color.FromArgb(255, 125, 150, 115)), new SolidBrush(Color.FromArgb(255, 145, 170, 135)) };

        protected static Brush[] BodyBrush = { new SolidBrush(Color.FromArgb(255, 115, 125, 125)), new SolidBrush(Color.FromArgb(255, 135, 145, 145)), new SolidBrush(Color.FromArgb(255, 115, 125, 125)), new SolidBrush(Color.FromArgb(255, 135, 145, 145)) };

        /// <summary>
        /// Creates a new node frame which can be drawn using the <see cref="Graphics"/>
        /// </summary>
        /// <param name="titleRect"><see cref="RectangleF"/> describing the title section size.</param>
        /// <param name="bodyRect"><see cref="RectangleF"/> describing the body section size. May or may not include the title area.</param>
        public NodeFrame(RectangleF titleRect, RectangleF bodyRect)
        {
            hoverRect = bodyRect;

            TitleFrame = CreateRoundedRect(Rectangle.Round(titleRect), 10, RoundedSide.Top);
            BodyFrame  = CreateRoundedRect(Rectangle.Round(bodyRect),  10, RoundedSide.Top | RoundedSide.Bottom);
        }

        public bool Contains(float x, float y)
        {
            return hoverRect.Contains(x, y);
        }

        public void Draw(Graphics g, SelectionMode mode)
        {
            uint brushId = (uint)(mode & (SelectionMode.Hover | SelectionMode.Selected));
            brushId = (uint)Math.Min(TitleBrush.Length - 1, brushId);
            brushId = (uint)Math.Min(BodyBrush.Length - 1, brushId);

            g.FillPath(BodyBrush[brushId],  BodyFrame);
            g.FillPath(TitleBrush[brushId], TitleFrame);

            if (mode == SelectionMode.None)
                return;

            if ((mode & SelectionMode.Error) == SelectionMode.Error)
            {
                g.DrawPath(new Pen(Color.Red, 5), BodyFrame);
                return;
            }

            if ((mode & SelectionMode.Warning) == SelectionMode.Warning)
            {
                g.DrawPath(new Pen(Color.Yellow, 4), BodyFrame);
                return;
            }

            if ((mode & SelectionMode.Selected) == SelectionMode.Selected)
            {
                g.DrawPath(new Pen(Color.Goldenrod, 2), BodyFrame);
                return;
            }

            g.DrawPath(new Pen(Color.WhiteSmoke, 1.5f), BodyFrame);
        }

        protected static GraphicsPath CreateRoundedRect(Rectangle bounds, int radius, RoundedSide side)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            var path = new GraphicsPath();

            if (radius == 0 || side == RoundedSide.None)
            {
                path.AddRectangle(bounds);
                return path;
            }

            bool hasTop = (side & RoundedSide.Top) == RoundedSide.Top;
            bool hasBottom = (side & RoundedSide.Bottom) == RoundedSide.Bottom;


            if (hasTop)
            {
                // top left arc  
                path.AddArc(arc, 180, 90);

                // top right arc  
                arc.X = bounds.Right - diameter;
                path.AddArc(arc, 270, 90);
            }
            else
            {
                // top left corner
                arc.X = bounds.Left + radius;
                path.AddLine(arc.X - radius, arc.Y + radius, arc.X - radius, arc.Y);
                path.AddLine(arc.X - radius, arc.Y, arc.X, arc.Y);

                // top right corner  
                arc.X = bounds.Right - radius;
                path.AddLine(arc.X, arc.Y, arc.X + radius, arc.Y);
                path.AddLine(arc.X + radius, arc.Y, arc.X + radius, arc.Y + radius);
            }

            if (hasBottom)
            {
                // bottom right arc  
                arc.X = bounds.Right - diameter;
                arc.Y = bounds.Bottom - diameter;
                path.AddArc(arc, 0, 90);

                // bottom left arc 
                arc.X = bounds.Left;
                path.AddArc(arc, 90, 90);
            }
            else
            {
                // bottom right arc  
                arc.X = bounds.Right - radius;
                arc.Y = bounds.Bottom - radius;
                path.AddLine(arc.X + radius, arc.Y, arc.X + radius, arc.Y + radius);
                path.AddLine(arc.X + radius, arc.Y + radius, arc.X, arc.Y + radius);

                // top right corner  
                arc.X = bounds.Left + radius;
                arc.Y = bounds.Bottom - radius;
                path.AddLine(arc.X, arc.Y + radius, arc.X - radius, arc.Y + radius);
                path.AddLine(arc.X - radius, arc.Y + radius, arc.X - radius, arc.Y);
            }

            path.CloseFigure();
            return path;
        }
    }
}
