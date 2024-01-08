using StudySpark.Core.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace StudySpark.Core {
    public class FontManager {
        public static ObservableCollection<GenericFontElement> GetAvailableFonts() {
            ObservableCollection<GenericFontElement> fontNames = new ObservableCollection<GenericFontElement>();

            foreach (FontFamily font in FontFamily.Families) {
                fontNames.Add(new GenericFontElement() { FontName = font.Name });
            }

            return fontNames;
        }
    }
}
