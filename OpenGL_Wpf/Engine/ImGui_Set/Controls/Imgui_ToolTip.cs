﻿using ImGuiNET;
using System;

namespace Simple_Engine.Engine.ImGui_Set.Controls
{
    internal class Imgui_ToolTip : ImgUI_Controls
    {
        public Imgui_ToolTip(ImgUI_Controls guiWindow, Func<bool> condition, string name) : base(guiWindow)
        {
            Condition = condition ?? throw new ArgumentNullException(nameof(condition));
            Name = name;
        }

        public Func<bool> Condition { get; set; }
        public string Name { get; set; }

        public override void BuildModel()
        {
            if (Condition())
            {
                ImGui.BeginTooltip();
                ImGui.SetTooltip(Name);
                ImGui.EndTooltip();
            }
        }

        public override void EndModel()
        {
        }
    }
}