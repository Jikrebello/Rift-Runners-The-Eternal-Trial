﻿using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Stride.Core.Mathematics;

namespace Test.Game_Logic.Camera
{
    public static class CameraSettings
    {
        public static float MinVerticalAngle { get; set; } = -10f;
        public static float MaxVerticalAngle { get; set; } = 70f;

        public static float VerticalSpeed { get; set; }
        public static float RotationSpeed { get; set; }
        public static bool InvertX { get; set; }
        public static bool InvertY { get; set; }
        public static Vector3 BaseCameraPosition { get; set; }
        public static Vector3 AimingCameraOffset { get; set; }
        public static float TransitionSpeed { get; set; }
        public static float DefaultFOV { get; set; }
        public static float AimingFOV { get; set; }

        private static FileSystemWatcher fileWatcher;
        private static readonly string filePath =
            @"D:\Stride Projects\Rift-Runners-The-Eternal-Trial\Test\Game Logic\Settings Files\CameraSettings.xml";

        static CameraSettings()
        {
            LoadSettingsFromXML(filePath);
            SetupFileWatcher(filePath);
        }

        private static void LoadSettingsFromXML(string filePath)
        {
            var xDoc = XDocument.Load(filePath);

            var settings = xDoc.Element("CameraSettings");

            if (settings != null)
            {
                VerticalSpeed = (float)Convert.ToDouble(settings.Element("VerticalSpeed")?.Value);
                RotationSpeed = (float)Convert.ToDouble(settings.Element("RotationSpeed")?.Value);
                InvertX = Convert.ToBoolean(settings.Element("InvertX")?.Value);
                InvertY = Convert.ToBoolean(settings.Element("InvertY")?.Value);

                var basePos = settings.Element("BaseCameraPosition");
                if (basePos != null)
                {
                    BaseCameraPosition = new Vector3(
                        (float)Convert.ToDouble(basePos.Attribute("x")?.Value),
                        (float)Convert.ToDouble(basePos.Attribute("y")?.Value),
                        (float)Convert.ToDouble(basePos.Attribute("z")?.Value)
                    );
                }

                var aimOffset = settings.Element("AimingCameraOffset");
                if (aimOffset != null)
                {
                    AimingCameraOffset = new Vector3(
                        (float)Convert.ToDouble(aimOffset.Attribute("x")?.Value),
                        (float)Convert.ToDouble(aimOffset.Attribute("y")?.Value),
                        (float)Convert.ToDouble(aimOffset.Attribute("z")?.Value)
                    );
                }

                TransitionSpeed = (float)
                    Convert.ToDouble(settings.Element("TransitionSpeed")?.Value);
                DefaultFOV = (float)Convert.ToDouble(settings.Element("DefaultFOV")?.Value);
                AimingFOV = (float)Convert.ToDouble(settings.Element("AimingFOV")?.Value);
            }
        }

        private static void SetupFileWatcher(string path)
        {
            fileWatcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(path),
                Filter = Path.GetFileName(path),
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
            };

            fileWatcher.Changed += OnChanged;
            fileWatcher.EnableRaisingEvents = true;
        }

        private static async void OnChanged(object sender, FileSystemEventArgs e)
        {
            fileWatcher.EnableRaisingEvents = false;

            await Task.Delay(500);

            LoadSettingsFromXML(filePath);

            fileWatcher.EnableRaisingEvents = true;
        }
    }
}
