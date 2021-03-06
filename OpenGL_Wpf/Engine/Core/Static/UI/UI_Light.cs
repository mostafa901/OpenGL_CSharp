﻿using ImGuiNET;
using OpenTK;
using Simple_Engine.Engine.GameSystem;
using Simple_Engine.Engine.Geometry.ThreeDModels;
using Simple_Engine.Engine.Illumination;
using Simple_Engine.Engine.Space.Scene;
using Simple_Engine.Extentions;

namespace Simple_Engine.Engine.Core.Static
{
    public static class UI_Light
    {
        private static LightModel light;

        public static void RenderUI(LightModel model)
        {
            if (model == null) return;
            light = model;

            RenderWindow();
        }

        private static bool isWindowOpen;

        private static void RenderWindow()
        {
            ImGui.SetNextWindowDockID(1);
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(250, Game.Instance.Height - UI_Game.TotalHeight));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0);
            if (ImGui.Begin("Light", ref isWindowOpen, ImGuiWindowFlags.DockNodeHost | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse))
            {
                UI_Shared.Render_Name(light);

                UI_Shared.Render_CastShadow(light);
                Render_Color();
                Render_Intenisty();
                Render_ShowLightRay();
                Render_Position();

                // Early out if the window is collapsed, as an optimization.
                ImGui.End();
            }
            ImGui.PopStyleVar();
        }

        private static void Render_Position()
        {
            var val = light.LightPosition.ToSystemNumeric();
            if (ImGui.DragFloat3("Position", ref val))
            {
                light.LightPosition = val.ToVector();
                light.SetShadowTransform();
                light.UpdateLightRay();
            }
        }

        private static void Render_ShowLightRay()
        {
            bool val = light.LightRay?.IsActive ?? false;
            if (ImGui.Checkbox("Show Light Ray", ref val))
            {
                if (light.LightRay == null)
                {
                    light.LightRay = new Line(new Vector3(), new Vector3(1));
                    light.LightRay.IsSystemModel = true;
                    light.LightRay.IsActive = false;
                    light.LightRay.BuildModel();
                    SceneModel.ActiveScene.UpLoadModels(light.LightRay);
                }

                {
                    light.LightRay.IsActive = !light.LightRay.IsActive;
                    if (light.LightRay.IsActive)
                    {
                        light.UpdateLightRay();
                    }
                }
            }
        }

        private static void Render_Intenisty()
        {
            float val = light.Intensity;
            float prev = val;
            UI_Shared.DragFloat("Intensity", ref val, ref prev, (x) =>
               {
                   light.DefaultColor += new Vector4(x, x, x, 0);
                   light.Intensity += x;
               }, 0, 10);
        }

        private static void Render_Color()
        {
            UI_Shared.Render_Color(light);
        }
    }
}