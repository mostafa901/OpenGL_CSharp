﻿using ImGuiNET;
using OpenTK;
using Simple_Engine.Engine.GameSystem;
using Simple_Engine.Engine.Space.Camera;
using Simple_Engine.Engine.Space.Scene;
using Simple_Engine.Extentions;
using System.Linq;

namespace Simple_Engine.Engine.Core.Static
{
    public static class UI_Camera
    {
        private static CameraModel camera;

        public static void RenderUI(CameraModel model)
        {
            if (model == null) return;
            camera = model;

            RenderWindow();
        }

        private static bool isWindowOpen;

        private static void RenderWindow()
        {
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(0, 25));

            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0);
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(250, Game.Instance.Height - UI_Game.TotalHeight));
            if (ImGui.Begin("Camera", ref isWindowOpen, ImGuiWindowFlags.DockNodeHost | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse))
            {
                ImGui.Text("Camera Settings");
                UI_Shared.Render_Name(camera);

                RenderDisplayMode();
                Render_FOV();
                if (!camera.IsPerspective)
                {
                    Render_Height();
                    Render_Width();
                }
                Render_Position();
                Render_Target();
                RenderCameraLine();
                Render_Test();
                Render_Clipping();

                Render_CameraList();

                // Early out if the window is collapsed, as an optimization.
                ImGui.End();
            }
            ImGui.PopStyleVar();
        }

        private static void Render_CameraList()
        {
            if (ImGui.Button("Save Camera"))
            {
                string cameraName = CameraModel.ActiveCamera.Name;
                if (string.IsNullOrEmpty(cameraName))
                {
                    UI_Shared.Render_Message("Name First", "Please add a name to the current camera");

                    return;
                }

                var newCamera = CameraModel.Create(SceneModel.ActiveScene, CameraModel.ActiveCamera.ViewType);
                newCamera.Name = cameraName;
                newCamera.Position = camera.Position;
                newCamera.Target = camera.Target;
                newCamera.UpdateCamera();
                SceneModel.ActiveScene.CameraModels.Add(newCamera);
            }

            if (SceneModel.ActiveScene.CameraModels.Any())
            {
                int selection = -1;
                string[] camnames = SceneModel.ActiveScene.CameraModels.Select(o => o.Name).ToArray();
                if (ImGui.ListBox("Saved Cameras", ref selection, camnames, camnames.Length))
                {
                    if (selection != -1)
                    {
                        CameraModel.ActiveCamera.UpdateViewTo(SceneModel.ActiveScene.CameraModels.ElementAt(selection));
                    }
                }
            }
        }

        private static void Render_Test()
        {
            if (ImGui.Button("Test"))
            {
                CameraModel.ActiveCamera.UpdateViewTo(CameraModel.PlanCamera);

                var pos = SceneModel.ActiveScene.BBX.GetCG() * new OpenTK.Vector3(1, 0, 1) + new OpenTK.Vector3(0, (float)20, 0);
                CameraModel.ClipPlanY.MoveTo(pos - (CameraModel.ClipPlanY.ClipDirection * 5));
                CameraModel.ClipPlanY.SetAsGlobal(true);
                CameraModel.ClipPlanY.IsActive = true;
            }
        }

        private static void Render_Clipping()
        {
            if (ImGui.Checkbox("Clip Model", ref CameraModel.EnableClipPlans))
            {
                if (!CameraModel.EnableClipPlans)
                {
                    foreach (var clip in CameraModel.ClipPlans)
                    {
                        clip.IsActive = false;
                    }
                }
            }

            if (CameraModel.EnableClipPlans)
            {
                ImGui.BeginGroup();
                foreach (var clip in CameraModel.ClipPlans)
                {
                    UI_Shared.Render_IsActive(clip);

                    ImGui.SameLine();

                    var prev = (clip.LocalTransform.ExtractTranslation() * clip.ClipDirection).Length;
                    var val = prev;

                    if (clip.IsActive)
                    {
                        UI_Shared.DragFloat(clip.Name, ref val, ref prev, (x) =>
                        {
                            clip.MoveLocal(clip.ClipDirection * x);
                        });
                    }
                    else
                    {
                        ImGui.TextDisabled("Disabled");
                    }
                }

                ImGui.EndGroup();
            }
        }

        private static void Render_FOV()
        {
            var val = camera.FOV;

            if (ImGui.DragFloat("FOV", ref val, .01f, 1f, 90))
            {
                camera.UpdateFOV(val);
            }
        }

        private static void Render_Position()
        {
            var pos = camera.Position.ToSystemNumeric();
            if (ImGui.DragFloat3("Position", ref pos, .01f))
            {
                camera.Position = pos.ToVector();
                camera.UpdateCamera();
            }
        }

        private static void Render_Target()
        {
            var pos = camera.Target.ToSystemNumeric();
            if (ImGui.DragFloat3("Target", ref pos, .01f))
            {
                camera.Target = pos.ToVector();
                camera.UpdateCamera();
            }
        }

        private static void RenderCameraLine()
        {
            ImGui.Checkbox("Show Camera Line", ref camera.IsDirectionVisible);
        }

        private static void RenderDisplayMode()
        {
            string mode = CameraModel.ActiveCamera.IsPerspective ? "Ortho" : "Perspective";
            if (ImGui.Button($"Activate {mode}"))
            {
                camera.IsPerspective = !camera.IsPerspective;
                if (camera.IsPerspective)
                {
                    camera.ActivatePrespective();
                }
                else
                {
                    camera.Activate_Ortho();
                }
            }
        }

        private static void Render_Height()
        {
            var val = camera.GetHeight();
            if (ImGui.DragFloat("Height", ref val))
            {
                camera.SetHeight(MathHelper.Clamp(val, 50, 2000));

                camera.UpdateViewMode();
            }
        }

        private static void Render_Width()
        {
            var val = camera.Width;
            if (ImGui.DragFloat("Width", ref val))
            {
                val = MathHelper.Clamp(val, 50, 2000);
                camera.SetHeight(val / 1.3f);
                camera.UpdateViewMode();
            }
        }
    }
}