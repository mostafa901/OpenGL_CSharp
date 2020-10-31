﻿using ImGuiNET;
using Simple_Engine.Engine.Core.Interfaces;
using Simple_Engine.Engine.GameSystem;
using Simple_Engine.Engine.Space.Scene;
using Simple_Engine.Extentions;
using System;
using System.Numerics;

namespace Simple_Engine.Engine.Core.Static
{
    public static class UI_Shared
    {
        static bool disableKey = false;
        public static bool OpenContext=false;
        public static bool IsAnyCaptured()
        {
            return ImGui.IsAnyItemHovered() || ImGui.IsWindowHovered(ImGuiHoveredFlags.AnyWindow) || !Game.Instance.Focused || disableKey;
        }

        public static void Render_YesNOModalMessage(string Name, string Message, Action<bool> ResponseAction)
        {
            bool isOpen = true;

            // Always center this window when appearing
            System.Numerics.Vector2 center = new System.Numerics.Vector2(ImGui.GetIO().DisplaySize.X * 0.5f, ImGui.GetIO().DisplaySize.Y * 0.5f);
            ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new System.Numerics.Vector2(0.5f, 0.5f));
            
            if (ImGui.BeginPopupModal(Name, ref isOpen, ImGuiWindowFlags.Modal| ImGuiWindowFlags.AlwaysAutoResize| ImGuiWindowFlags.NoResize))
            {
                disableKey = true;
                ImGui.Text(Message);
                ImGui.Separator();

                if (ImGui.Button("Yes", new System.Numerics.Vector2(120, 0)))
                {
                    ResponseAction(true);
                    ImGui.CloseCurrentPopup();
                    disableKey = isOpen = false;
                }
                ImGui.SetItemDefaultFocus();
                ImGui.SameLine();
                if (ImGui.Button("Cancel", new System.Numerics.Vector2(120, 0)))
                {
                    ResponseAction(false);
                    ImGui.CloseCurrentPopup();
                    disableKey = isOpen = false;
                }

                ImGui.EndPopup();
            }
        }

        public static bool isInputChanged(string name, ref string val)
        {
            return ImGui.InputText(name, ref val, 200);
        }

        public static bool IsExpanded(string name)
        {
            return ImGui.CollapsingHeader(name, ImGuiTreeNodeFlags.OpenOnArrow);
        }

        public static void Render_Color(IRenderable Model)
        {
            var InitialValue = Model.DefaultColor.ToSystemNumeric();
            if (ImGui.ColorPicker4("##picker", ref InitialValue))
            {
                Model.DefaultColor = InitialValue.ToVector().Round(2);

                if (Model is IDrawable)
                {
                    var drawable = Model as IDrawable;
                    drawable.ShaderModel.RunOnUIThread.Push(() =>
                     {
                         drawable.ShaderModel.SetVector4(drawable.ShaderModel.Location_DefaultColor, drawable.DefaultColor);
                     });
                }
            }
        }

        internal static void DragFloat(string name, ref float val, ref float prev, Action<float> p)
        {
            if (ImGui.DragFloat(name, ref val))
            {
                p(val - prev);
            }

            if (ImGui.IsItemHovered())
            {
                var wh = ImGui.GetIO().MouseWheel;
                if (ImGui.GetIO().KeyCtrl)
                {
                    wh *= 2;
                }
                if (ImGui.GetIO().KeyAlt)
                {
                    wh /= 2;
                }
                p(wh * .01f);
            }
        }

        public static void Render_Name(IRenderable model)
        {
            string val = model.Name ?? "";
            if (UI_Shared.isInputChanged("Name", ref val))
            {
                model.Name = val;
            }
        }

        public static void Render_Progress(float progress, float max, string message)
        {
            bool open = true;
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(Game.Instance.Width / 2, Game.Instance.Height / 2), ImGuiCond.Always, new System.Numerics.Vector2(.5f, .5f));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 20);
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new System.Numerics.Vector2());
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(400, 0));
            ImGui.SetNextWindowContentSize(new System.Numerics.Vector2(350, 0));

            if (ImGui.Begin("Progress", ref open, ImGuiWindowFlags.Modal | ImGuiWindowFlags.NoTitleBar))
            {
                progress = progress / max;
                ImGui.ProgressBar(progress, new System.Numerics.Vector2(0.0f, 0.0f));

                ImGui.Text(message);

                ImGui.End();
            }
        }

        public static void Render_CastShadow(IRenderable model)
        {
            var val = model.CastShadow;
            if (ImGui.Checkbox("CastShadow", ref val))
            {
                model.CastShadow = val;
            }
        }

        public static void Render_Isolate(IDrawable model)
        {
            Action isolate = () =>
            {
                SceneModel.ActiveScene.IsolateDisplay = !SceneModel.ActiveScene.IsolateDisplay;
                if (SceneModel.ActiveScene.IsolateDisplay)
                {
                    SceneModel.ActiveScene.IsolateModel(model);
                }
                else
                {
                    SceneModel.ActiveScene.ActivateModels();
                }
            };

            if (SceneModel.ActiveScene.IsolateDisplay)
            {
                ImGui.PushID("Red");
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(1, 0, 0, 1));
                if (ImGui.Button("Isolate is Active"))
                {
                    isolate();
                }
                ImGui.PopStyleColor();
                ImGui.PopID();
            }
            else
            {
                if (ImGui.Button("Isolate"))
                { isolate(); }
            }
        }

        private static void ColorButton(string styleName, string buttonNAme)
        {
        }
    }
}