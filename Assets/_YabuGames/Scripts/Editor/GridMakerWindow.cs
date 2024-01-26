using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace _YabuGames.Scripts.Editor
{
    public class GridMakerWindow : EditorWindow
    {
        private int _horizontalGridCount;
        private int _verticalGridCount;
        private float _space;
        private GameObject _desiredObject;
        private List<GameObject> _gridList;

        [MenuItem("Window/Yabu Tools/Grid Maker")]
        private static void ShowWindow()
        {
            var window = GetWindow<GridMakerWindow>();
            window.titleContent = new GUIContent("Grid Maker");
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("                     YABU GAMES",EditorStyles.largeLabel);
            _horizontalGridCount = EditorGUILayout.IntField("Horizontal Grid Count ", _horizontalGridCount);
            _verticalGridCount = EditorGUILayout.IntField("Vertical Grid Count ", _verticalGridCount);
            _space = EditorGUILayout.FloatField("Space  between objects", _space);
            GUILayout.Space(10);
            GUILayout.Label("Select an object that you want to create a grid with it.",EditorStyles.boldLabel);
            if (GUILayout.Button("CREATE"))
            {
                CreateGrid();
            }
        }

        private void CreateGrid()
        {
            _gridList.Clear();
            if (Selection.gameObjects.Length < 1 || Selection.gameObjects.Length > 1)
            {
                Debug.LogWarning("Please Select Only 1 GameObject!");
                return;
            }
            if (_horizontalGridCount < 2)
            {
                Debug.LogWarning("Grid Size Must Be Higher Than 1!");
                return;
            }

            var parent = new GameObject
            {
                name = $"Grid Root ({_horizontalGridCount + "x" + _verticalGridCount})"
            };
            var origin = Vector3.zero;
            _desiredObject = Selection.gameObjects[0];
            var referenceScale = _desiredObject.transform.localScale;
            var calculatedSpaceX = _space + referenceScale.x;
            var calculatedSpaceZ = _space + referenceScale.z;
           
            for (var i = 0; i < _horizontalGridCount; i++)
            {
                //To Right
                origin += Vector3.right * calculatedSpaceX;
                var obj = Instantiate(_desiredObject, origin, Quaternion.identity);
                var desiredPos = obj.transform.position;
                obj.name = "Grid";
                _gridList.Add(obj);
                
                for (var j = 0; j < _verticalGridCount-1; j++)
                {
                    // To Back
                    desiredPos += Vector3.back * calculatedSpaceZ;
                    var newObj = Instantiate(_desiredObject, desiredPos, Quaternion.identity);
                    newObj.name = "Grid";
                    _gridList.Add(newObj);
                }
            }

            foreach (var grid in _gridList)
            {
                grid.transform.SetParent(parent.transform);
            }
        }
    }
}