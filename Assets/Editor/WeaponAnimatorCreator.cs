using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Project.Weapons
{
    public class WeaponAnimatorCreator
    {
        private const string DEFAULT_PATH = "Assets/Animation/AnimatorControllers/Combat";

        [MenuItem("Project/Weapon System/New Animator")]
        static void CreateNewWeaponAnimator()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "") path = DEFAULT_PATH;
            
            var pathComps = path.Split('/').ToList();
            
            foreach (string comp in pathComps.ToList())
            {
                if (comp.Contains('.'))
                {
                    pathComps.Remove(comp);
                }
            }
            
            pathComps.Add("Base_Weapon_Animator.controller");

            path = string.Join('/', pathComps);
            
            var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(path);
            
            var labels = AssetDatabase.GetLabels(controller).ToList();
            
            labels.Add("Animator");
            labels.Add("Weapon");
            
            AssetDatabase.SetLabels(controller, labels.ToArray());
            
            controller.AddParameter("active", AnimatorControllerParameterType.Bool);
            controller.AddParameter("hold", AnimatorControllerParameterType.Bool);
            controller.AddParameter("cancel", AnimatorControllerParameterType.Bool);
            controller.AddParameter("counter", AnimatorControllerParameterType.Int);

            var rootStateMachine = controller.layers[0].stateMachine;
            var emptyState = rootStateMachine.AddState("Empty");
            rootStateMachine.defaultState = emptyState;
            var exitTransition = emptyState.AddExitTransition();
            exitTransition.duration = 0f;
            exitTransition.exitTime = 0f;
            exitTransition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 1, "active");
        }
    }
}