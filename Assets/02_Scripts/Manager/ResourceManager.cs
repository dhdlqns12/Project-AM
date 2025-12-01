using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public  static class ResourceManager
{
   private static Dictionary<string, TextAsset> buildingTextAsset;
   private static Dictionary<string, TextAsset> unitTextAsset;
   private static Dictionary<string, TextAsset> enemyTextAsset;
   //해당 필드들에 미리 데이터 로드 후 호출할 시에는 바로 보내주는 식으로
   
   /// <summary>
   /// 모든 리소스(프리팹, 스프라이트, 사운드 등)를 로드하는 공통 매서드
   /// </summary>
   /// <param name="path"></param>
   /// <typeparam name="T"></typeparam>
   /// <returns></returns>
   public static T LoadAsset<T>(string path) where T : Object
   {
      T asset = Resources.Load<T>(path);
      if (asset == null)
      {
         Debug.LogError("해당 경로에서 리소스를 로드할 수 없습니다.");
      }
      return asset;
   }

   
   
   /// <summary>
   /// Json파일을 로드하여 T타입의 단일 객체를 반환하는 매서드
   /// </summary>
   /// <param name="path"></param>
   /// <typeparam name="T"></typeparam>
   /// <returns></returns>
   public static T LoadJsonData<T>(string path)
   {
      TextAsset textAsset = LoadAsset<TextAsset>(path);
      if (textAsset == null)
      {
        return default;
      }
      return JsonConvert.DeserializeObject<T>(textAsset.text);
   }
   
   
   
   /// <summary>
   /// Json파일을 로드하여 T타입 리스트/배열로 반환하는 매서드
   /// </summary>
   /// <param name="path"></param>
   /// <typeparam name="T"></typeparam>
   /// <returns></returns>
   public static T[] LoadJsonDataList<T>(string path)
   {
      TextAsset textAsset = LoadAsset<TextAsset>(path);
      if (textAsset == null)
      {
         Debug.LogError("해당 경로에서 JsonData를 로드할 수 없습니다.");
         return null;
      }
      return JsonConvert.DeserializeObject<T[]>(textAsset.text);
   }
   
}
