using UnityEditor;

namespace RoboRiftRush
{
    public static class CreateScriptTemplates
    {
        [MenuItem("Assets/Create/Scripts/MonoBehaviour", priority = 40)]
        public static void CreateMonoBehaviorMenuItem() {
            string templatePath = "Assets/Editor/Templates/NewBehaviourScript.cs.txt";

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewScript.cs");
        }

        [MenuItem("Assets/Create/Scripts/ScriptableObjectScript", priority = 41)]
        public static void CreateScriptableObjectScriptMenuItem() {
            string templatePath = "Assets/Editor/Templates/NewScriptableObjectScript.cs.txt";

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewScriptableObjectScript.cs");
        }

        [MenuItem("Assets/Create/Scripts/InterfaceScript", priority = 42)]
        public static void CreateInterfaceScriptMenuItem() {
            string templatePath = "Assets/Editor/Templates/NewInterfaceScript.cs.txt";

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewInterfaceScript.cs");
        }
    }
}
