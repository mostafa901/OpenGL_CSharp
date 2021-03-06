﻿using OpenTK;
using System.Linq;
using System.Xml;

namespace Simple_Engine.Engine.Fonts.Core
{
    public static partial class FontFactory
    {
        private static FontInfo finfo;

        public static string imgFontPath;
        private static XmlDocument xmldoc;

        public static void GenerateFont()
        {
            string fntPath = @"./Engine/Fonts/Render/Source/TextMap/arial_regular_90.fnt";
            imgFontPath = @"./Engine/Fonts/Render/Source/TextMap/arial_regular_90.png";

            xmldoc = new XmlDocument();
            xmldoc.Load(fntPath);

            var xeleinfo = xmldoc.DocumentElement.GetElementsByTagName("info").Item(0) as XmlElement;
            var xelecommon = xmldoc.DocumentElement.GetElementsByTagName("common").Item(0) as XmlElement;

            finfo = new FontInfo();
            finfo.ImgPath = imgFontPath;
            finfo.baseValue = int.Parse(xelecommon.GetAttribute("base"));
            finfo.ImgHeight = float.Parse(xelecommon.GetAttribute("scaleH"));
            finfo.ImgWidth = float.Parse(xelecommon.GetAttribute("scaleW"));
            finfo.LineHeight = int.Parse(xelecommon.GetAttribute("lineHeight"));
            finfo.Name = xeleinfo.GetAttribute("face");

            int[] padds = xeleinfo.GetAttribute("padding").Split(',').Select(o => int.Parse(o)).ToArray();
            finfo.Padding = new Vector4(padds[0], padds[1], padds[2], padds[3]);

            finfo.Size = float.Parse(xeleinfo.GetAttribute("size"));
        }

        public static CharacterModel GetCharacterModel(char c, int fontsize)
        {
            var xchars = xmldoc.GetElementsByTagName("char");
            foreach (XmlElement xchar in xchars)
            {
                var fchar = new CharacterModel(finfo, fontsize);
                fchar.Id = int.Parse(xchar.GetAttribute("id"));
                if (((char)fchar.Id).Equals(c))
                {
                    fchar.Advance = int.Parse(xchar.GetAttribute("xadvance"));
                    fchar.SetHeight(float.Parse(xchar.GetAttribute("height")));
                    fchar.SetWidth(float.Parse(xchar.GetAttribute("width")));
                    fchar.X = int.Parse(xchar.GetAttribute("x"));
                    fchar.Y = int.Parse(xchar.GetAttribute("y"));
                    fchar.YOffset = int.Parse(xchar.GetAttribute("yoffset"));
                    fchar.XOffset = int.Parse(xchar.GetAttribute("xoffset"));

                    return fchar;
                }
            }

            return null;
        }
    }
}