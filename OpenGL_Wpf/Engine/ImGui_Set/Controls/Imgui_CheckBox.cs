﻿using ImGuiNET;
using System;

namespace Simple_Engine.Engine.ImGui_Set.Controls
{
    internal class Imgui_CheckBox : ImgUI_Controls
    {
        public Imgui_CheckBox(ImgUI_Controls guiWindow, string name, Func<bool> initialValue, Action<bool> buttonAction) : base(guiWindow)
        {
            Name = name;
            InitialValue = initialValue;
            ButtonAction = buttonAction;
            Width = 150;
        }

        private Func<bool> InitialValue;
        public Action<bool> ButtonAction { get; }

        public override void BuildModel()
        {
            var val = InitialValue();
            if (ImGui.Checkbox(Name, ref val))
            {
                ButtonAction(val);
            }
        }

        public override void EndModel()
        {
        }
    }
}