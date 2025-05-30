﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DXFLibrary
{
    public class Ellipse : Entity
    {
        private Point center = new Point();
        public Point Center { get { return center; } }

        private Point mainaxis = new Point();
        public Point MainAxis { get { return mainaxis; } }

        private Point extrusion = new Point();
        public Point ExtrusionDirection { get { return extrusion; } }

        public double AxisRatio { get; set; }

        public double StartParam { get; set; }
        public double EndParam { get; set; }

        public override void ParseGroupCode(int groupcode, string value)
        {
            base.ParseGroupCode(groupcode, value);
            switch (groupcode)
            {
                case 10:
                    Center.X = double.Parse(value);
                    break;
                case 20:
                    Center.Y = double.Parse(value);
                    break;
                case 30:
                    Center.Z = double.Parse(value);
                    break;
                case 11:
                    MainAxis.X = double.Parse(value);
                    break;
                case 21:
                    MainAxis.Y = double.Parse(value);
                    break;
                case 31:
                    MainAxis.Z = double.Parse(value);
                    break;
                case 210:
                    ExtrusionDirection.X = double.Parse(value);
                    break;
                case 220:
                    ExtrusionDirection.Y = double.Parse(value);
                    break;
                case 230:
                    ExtrusionDirection.Z = double.Parse(value);
                    break;
                case 40:
                    AxisRatio = double.Parse(value);
                    break;
                case 41:
                    StartParam = double.Parse(value);
                    break;
                case 42:
                    EndParam = double.Parse(value);
                    break;
            }
        }
    }
}
