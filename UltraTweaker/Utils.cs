using BepInEx;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace UltraTweaker
{
    public static class PathUtils
    {
        public static string GameDirectory()
        {
            string path = Application.dataPath;
            if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                path = Utility.ParentDirectory(path, 2);
            }
            else if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                path = Utility.ParentDirectory(path, 1);
            }

            return path;
        }

        public static string ModPath(Assembly asm = null)
        {
            if (asm == null)
            {
                asm = Assembly.GetExecutingAssembly();
            }

            return asm.Location.Substring(0, asm.Location.LastIndexOf(@"\"));
        }
    }

    public static class GameObjectUtils
    {
        public static GameObject ChildByName(this GameObject from, string name)
        {
            List<GameObject> children = new();
            int count = 0;
            while (count < from.transform.childCount)
            {
                children.Add(from.transform.GetChild(count).gameObject);
                count++;
            }

            if (count == 0)
            {
                return null;
            }

            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].name == name)
                {
                    return children[i];
                }
            }
            return null;
        }

        public static List<GameObject> FindSceneObjects(string sceneName)
        {
            List<GameObject> objs = new();
            foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
            {
                if (obj.scene.name == sceneName)
                {
                    objs.Add(obj);
                }
            }

            return objs;
        }

        public static List<GameObject> ChildrenList(this GameObject from)
        {
            List<GameObject> children = new();
            int count = 0;
            while (count < from.transform.childCount)
            {
                children.Add(from.transform.GetChild(count).gameObject);
                count++;
            }

            return children;
        }
    }

    public class UltrakillUtils
    {
        public static GameObject NearestEnemy(Vector3 point, float maxDistance)
        {
            float max = maxDistance;
            GameObject nearestEnemy = null;

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (GameObject enemy in enemies)
            {
                if (enemy.GetComponent<EnemyIdentifier>() != null && !enemy.GetComponent<EnemyIdentifier>().dead && Vector3.Distance(point, enemy.transform.position) < max)
                {
                    max = Vector3.Distance(point, enemy.transform.position);
                    nearestEnemy = enemy;
                }
            }

            return nearestEnemy;
        }
    }

    public class DisableDoubleRender : MonoBehaviour
    {
        private EnemyIdentifier eid;
        private DoubleRender[] dr;

        public void Start()
        {
            eid = null;
            dr = null;
        }

        public void Update()
        {
            if (dr == null)
            {
                if (dr == null)
                {
                    dr = GetComponentsInChildren<DoubleRender>(true);
                }
            }

            if (dr != null && dr.Length > 0)
            {
                foreach (DoubleRender dr_ in dr)
                {
                    dr_.currentCam.RemoveCommandBuffer(UnityEngine.Rendering.CameraEvent.BeforeForwardAlpha, dr_.cb);
                }
            }
        }
    }
}