﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.UI;
using System.Diagnostics;
using Microsoft.Toolkit.Uwp;

namespace ColorfulBox
{
    public class ComplementaryStrategy : ColorPaletteStrategy
    {
        public ComplementaryStrategy()
        {
        }

        private bool _isColorsChanging;

        public override void ChangeColorPoints(IList<ColorPoint> colorPoints)
        {
            _isColorsChanging = true;
            try
            {
                var primaryColorPoint = colorPoints.FirstOrDefault(p => p.IsPrimary);
                if (primaryColorPoint == null)
                    return;

                var primaryHsv = primaryColorPoint.HsvColor;
                primaryHsv.S = 0.76;
                var primatyIndex = colorPoints.IndexOf(primaryColorPoint);
                for (int i = 0; i < colorPoints.Count; i++)
                {
                    var colorPoint = colorPoints[i];
                    if (i == primatyIndex)
                    {
                        colorPoint.HsvColor = primaryHsv;//.Color =ColorExtensions.FromHsvEx(primaryHsv.H, primaryHsv.S, primaryHsv.V);
                    }
                    else
                    {
                        var hue = primaryHsv.H;

                        if (i > primatyIndex)
                        {
                            hue += 180;
                            if (hue > 360)
                                hue -= 360;
                        }
                        var value = primaryHsv.V;
                        var saturation = primaryHsv.S;
                        if (Math.Abs(i - primatyIndex) > 1)
                        {
                            value -= (Math.Abs(i - primatyIndex) - 1) * 0.3;
                            value = Math.Max(0, value);
                        }

                        saturation *= 1 + Math.Abs(i - primatyIndex) * 0.15;
                        saturation = Math.Min(1, saturation);

                        colorPoint.HsvColor=new HsvColor{ A= 1,H=hue,S=saturation,V=value};//  .Color = ColorExtensions.FromHsvEx(hue, saturation, value);
                    }
                }
            }
            finally
            {
                _isColorsChanging = false;
            }
        }

        public override void OnColorChanged(ColorPoint colorPoint, HsvColor oldColor, IList<ColorPoint> colorPoints)
        {
            base.OnColorChanged(colorPoint, oldColor, colorPoints);
            if (_isColorsChanging)
                return;

            _isColorsChanging = true;
            try
            {
                var primaryColorPoint = colorPoints.FirstOrDefault(p => p.IsPrimary);
                if (primaryColorPoint == null)
                    return;

                var primaryHsv = primaryColorPoint.HsvColor;
                var primatyIndex = colorPoints.IndexOf(primaryColorPoint);
                var colorPointHsv = colorPoint.HsvColor;
                var colorPointIndex = colorPoints.IndexOf(colorPoint);
                if (primaryColorPoint != colorPoint)
                {
                    if (colorPointIndex > primatyIndex)
                    {
                        primaryHsv.H = colorPointHsv.H + 180;
                        if (primaryHsv.H > 360)
                            primaryHsv.H -= 360;
                    }
                    else
                    {
                        primaryHsv.H = colorPointHsv.H;
                    }
                }
                var oldHsv = oldColor;
                for (int i = 0; i < colorPoints.Count; i++)
                {

                    var hue = primaryHsv.H;
                    var point = colorPoints[i];
                    var pointHsv = point.HsvColor;
                    if (i > primatyIndex)
                    {
                        hue += 180;
                        if (hue > 360)
                            hue -= 360;
                    }

                    var saturation = pointHsv.S;
                    if (colorPoint == primaryColorPoint)
                    {
                        if (i == primatyIndex)
                            continue;

                        var saturationRate =  pointHsv.S/ oldHsv.S;
                        if (pointHsv.S == 1)
                        {
                            saturationRate = 1 + Math.Abs(i - primatyIndex) * 0.15;
                        }
                        
                        saturation = saturationRate* primaryHsv.S  ;
                        saturation = Math.Min(1, saturation);
                    }
                  
                    point.HsvColor = new HsvColor {A = 1, H = hue, S = saturation, V = pointHsv.V};
                }
            }
            finally
            {
                _isColorsChanging = false;
            }

        }
    }
}
