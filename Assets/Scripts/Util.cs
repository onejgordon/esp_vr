using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

 public static class Util
 {
   public static double timestamp()
   {
     // In seconds
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        return (System.DateTime.UtcNow - epochStart).TotalMilliseconds / 1000.0;
   }

   public static void SetImage(GameObject target, string image_name) {
     Sprite s = Resources.Load<Sprite>(image_name);
     target.GetComponent<SpriteRenderer>().sprite = s;
   }

  public static List<T> Shuffle<T>(List<T> _list)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            T temp = _list[i];
            int randomIndex = Random.Range(i, _list.Count);
            _list[i] = _list[randomIndex];
            _list[randomIndex] = temp;
        }

        return _list;
    }

 }