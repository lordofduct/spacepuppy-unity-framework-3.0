using UnityEngine;
using System.Collections.Generic;

namespace com.spacepuppy.SPInput.Unity.Xbox
{

    public static class XboxInputFactory
    {

        public static InputLayoutToken<XboxInputId> LoadCustomLayout(string joystickName)
        {
            string path = System.IO.Path.Combine(Application.persistentDataPath, GetJoystickConfigFileName(joystickName));
            if (System.IO.File.Exists(path))
            {
                using (var strm = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                using (var serializer = com.spacepuppy.Serialization.SPSerializer.Create())
                {
                    var formatter = new com.spacepuppy.Serialization.Json.JsonFormatter();
                    return serializer.Deserialize(formatter, strm) as InputLayoutToken<XboxInputId>;
                }
            }

            return null;
        }

        public static void CommitCustomLayout(ConfigurableXboxInputProfile profile)
        {
            if (profile == null) return;
            if (string.IsNullOrEmpty(profile.Id)) return;

            if (profile.ContainsCustomLayout)
            {
                var token = new InputLayoutToken<XboxInputId>();
                profile.ApplyToToken(token);
                SaveCustomLayout(profile.Id, token);
            }
            else
            {
                DeleteCustomLayout(profile.Id);
            }
        }

        public static void SaveCustomLayout(string joystickName, InputLayoutToken<XboxInputId> token)
        {
            if (token == null) return;

            string path = System.IO.Path.Combine(Application.persistentDataPath, GetJoystickConfigFileName(joystickName));
            using (var strm = new System.IO.MemoryStream())
            using (var serializer = com.spacepuppy.Serialization.SPSerializer.Create())
            {
                var formatter = new com.spacepuppy.Serialization.Json.JsonFormatter();
                serializer.Serialize(formatter, strm, token);

                using (var file = new System.IO.FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    strm.Position = 0;
                    var arr = strm.ToArray();
                    file.Write(arr, 0, arr.Length);
                }
            }
        }

        public static void DeleteCustomLayout(string joystickName)
        {
            string path = System.IO.Path.Combine(Application.persistentDataPath, GetJoystickConfigFileName(joystickName));
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

        private static string GetJoystickConfigFileName(string joystickName)
        {
            return "custom-input-(" + joystickName + ").json";
        }

    }

}