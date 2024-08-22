using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using unvell.ReoGrid.Graphics;

namespace TestReoGrid.Helpers
{
    public static class ReoGridExtensions
    {
        /// <summary>
        /// Converts a Color to an OxyColor.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>An OxyColor.</returns>
        public static SolidColor ToReoColor(this Color color)
        {
            return SolidColor.FromArgb(color.A, color.R, color.G, color.B);
        }

        /// <summary>
        /// Converts a <see cref="Brush" /> to an <see cref="OxyColor" />.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <returns>An <see cref="OxyColor" />.</returns>
        public static SolidColor ToReoColor(this Brush brush)
        {
            var scb = brush as SolidColorBrush;
            return scb != null ? scb.Color.ToReoColor() : SolidColor.Transparent;
        }

    }
}
