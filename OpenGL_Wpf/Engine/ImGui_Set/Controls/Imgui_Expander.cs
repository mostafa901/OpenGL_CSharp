﻿using ImGuiNET;
using System.Linq;

namespace Simple_Engine.Engine.ImGui_Set.Controls
{
    internal class Imgui_Expander : ImgUI_Controls
    {
        public Imgui_Expander(ImgUI_Controls guiWindow, string name) : base(guiWindow)
        {
            Name = name;
            Width = 150;
        }

        public bool Expanded = true;

        public override void BuildModel()
        {
            if (ImGui.CollapsingHeader(Name, ImGuiTreeNodeFlags.None))
            {
                for (int i = 0; i < SubControls.Count(); i++)
                {
                    var ctrl = SubControls.ElementAt(i);
                    ctrl.BuildModel();
                }
            }
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(400, 0), ImGuiCond.Always);
        }

        public override void EndModel()
        {
        }
    }
}