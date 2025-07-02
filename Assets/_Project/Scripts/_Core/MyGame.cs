using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public static class Utils
    {
        // Default color constants for logging
        private const string Yellow = "#FFFF00"; // System comments
        private const string Green = "#00B300";  // Game comments
        private const string Red = "#FF0000";    // Test comments

        /// <summary>
        /// Logs a message with a custom color to the Unity console.
        /// </summary>
        /// <param name="text">The text to log.</param>
        /// <param name="color">The color of the text (optional, defaults to yellow).</param>
        public static void LogCustom(string text, string color = Yellow)
        {
            Log(text, color);
        }

        /// <summary>
        /// Logs a system comment in yellow.
        /// </summary>
        /// <param name="text">The text to log.</param>
        public static void SystemComment(string text)
        {
            Log(text, Yellow);
        }

        /// <summary>
        /// Logs a game-related comment in green.
        /// </summary>
        /// <param name="text">The text to log.</param>
        public static void GameComment(string text)
        {
            Log(text, Green);
        }

        /// <summary>
        /// Logs a test comment in red.
        /// </summary>
        /// <param name="text">The text to log.</param>
        public static void TestComment(string text)
        {
            Log(text, Red);
        }

        /// <summary>
        /// Logs a message with a specified color to the Unity console.
        /// </summary>
        /// <param name="text">The text to log.</param>
        /// <param name="color">The color of the text.</param>
        private static void Log(string text, string color)
        {
            Debug.Log($"<color={color}>{text}</color>");
        }

        /// <summary>
        /// Checks if a component exists, assigns it, and logs a message if not found.
        /// </summary>
        /// <typeparam name="T">The type of component to check for.</typeparam>
        /// <param name="gameObject">The GameObject to check for the component.</param>
        /// <param name="component">The component to assign if found.</param>
        /// <param name="scriptName">The name of the script where the check is being made.</param>
        /// <param name="color">The color of the message if the component is not found (optional, defaults to yellow).</param>
        /// <returns>Returns true if the component is found and assigned; otherwise, false.</returns>
        public static bool AddComponent<T>(out T component, GameObject gameObject, string scriptName, string color = Yellow) where T : Component
        {
            if (gameObject.TryGetComponent<T>(out component))
            {
                return true; // Component found
            }
            else
            {
                Log($"The component '{typeof(T).Name}' for '{gameObject.name}' in script '{scriptName}' not found!", color);
                component = null; // Ensure component is null if not found
                return false; // Component not found
            }
        }
    }

    public static class Datas
    {
        /// <summary>
        /// Randomizes the order of elements in a one-dimensional array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">The array to shuffle.</param>
        public static void ShuffleArray<T>(T[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                T temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }

        /// <summary>
        /// Randomizes the order of elements in a list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list.</typeparam>
        /// <param name="list">The list to shuffle.</param>
        public static void ShuffleList<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        /// <summary>
        /// Randomizes the order of elements in a two-dimensional array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">The two-dimensional array to shuffle.</param>
        public static void Shuffle2DArray<T>(T[,] array)
        {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);
            List<T> flattenedList = new List<T>();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    flattenedList.Add(array[i, j]);
                }
            }

            ShuffleList(flattenedList);

            int index = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    array[i, j] = flattenedList[index++];
                }
            }
        }
    }

    public static class Math
    {
        /// <summary>
        /// Calculates the squared distance between two points in 2D space.
        /// </summary>
        /// <param name="position1">The first position.</param>
        /// <param name="position2">The second position.</param>
        /// <returns>The squared distance between the two points in 2D.</returns>
        public static float Distance2D(Vector3 position1, Vector3 position2)
        {
            Vector2 pos1 = new Vector2(position1.x, position1.y);
            Vector2 pos2 = new Vector2(position2.x, position2.y);
            return (pos1 - pos2).sqrMagnitude;
        }

        /// <summary>
        /// Calculates the squared distance between two points in 3D space.
        /// </summary>
        /// <param name="position1">The first position.</param>
        /// <param name="position2">The second position.</param>
        /// <returns>The squared distance between the two points in 3D.</returns>
        public static float Distance3D(Vector3 position1, Vector3 position2)
        {
            return (position1 - position2).sqrMagnitude;
        }
        
        /// <summary>
        /// Rotates a transform to face a specific point in world space.
        /// </summary>
        /// <param name="targetTransform">The transform to rotate.</param>
        /// <param name="worldPosition">The target point in world space.</param>
        /// <param name="offsetDegree">An optional offset in degrees to adjust the final rotation angle (optional, defaults to 0f).</param>
        /// <param name="rotationSpeed">The speed of rotation (optional, defaults to 25f).</param>
        public static void Rotation(Transform targetTransform, Vector3 worldPosition, float offsetDegree = 0f, float rotationSpeed = 25f)
        {
            Vector3 direction = worldPosition - targetTransform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + offsetDegree;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            targetTransform.rotation = Quaternion.Slerp(targetTransform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }
    }
}