using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace _YabuGames.Scripts.Editor
{
    public class ColorizerWindow : EditorWindow
    {
        private Color _color;

        [MenuItem("Window/Yabu Tools/Colorizer")]
        public static void ShowWindow()
        {
            GetWindow<ColorizerWindow>("COLORIZER");
        }

        private void OnGUI()
        {
            GUILayout.Label("                     YABU GAMES",EditorStyles.largeLabel);
            GUILayout.Label("Color the selected objects", EditorStyles.boldLabel);
            _color = EditorGUILayout.ColorField("Desired Color",_color);
            Colorize();
        }

        private void Colorize()
        {
            if (GUILayout.Button("Colorize"))
            {
                foreach (var obj in Selection.objects)
                {
                    var renderer = obj.GetComponent<Renderer>();
                    if (renderer)
                    {
                        renderer.sharedMaterial.color = _color;
                    }
                }
            }
        }
    }
}
