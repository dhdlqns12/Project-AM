using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public  static class ResourceManager
{
   private static Dictionary<string, TextAsset> textAssets = new ();

   /// <summary>
   /// Resource매니저 초기화 로직
   /// TextAsset에 대한 정보를 초기화합니다.
   /// Resources 폴더의 데이터들을 미리 로드합니다.
   /// Resources 폴더 업데이트와 함께 수정 예정
   /// </summary>
   public static void Init()
   {
      Clear();
      PreLoadData();
      Debug.Log("리소스 매니저 준비 완료");
   }

   private static void Clear()
   {
      textAssets.Clear();
   }

   private static void PreLoadData()
   {
      var jsons = Resources.LoadAll<TextAsset>("Data");
      foreach (var kvp in jsons)
      {
         textAssets[kvp.name] = kvp;
         Debug.Log(kvp.name+ " Json데이터 로드 완료");
      }
   }
   
   
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
      textAssets.TryGetValue(path, out TextAsset json);

      if (json == null)
      {
         Debug.Log($"해당 경로({path})에서 json을 로드할 수 없습니다. ");
         return default;
      }
      
      return JsonConvert.DeserializeObject<T>(json.text);
   }
   
   
   
   /// <summary>
   /// Json파일을 로드하여 T타입 리스트/배열로 반환하는 매서드
   /// 매개변수 path에 Resources폴더 내의 Json파일 명을 정확하게 입력해주세요.
   /// </summary>
   /// <param name="path"></param>
   /// <typeparam name="T"></typeparam>
   /// <returns></returns>
   public static T[] LoadJsonDataList<T>(string path)
   {
      textAssets.TryGetValue(path, out TextAsset json);

      if (json == null)
      {
         Debug.Log($"해당 경로({path})에서 json을 로드할 수 없습니다. ");
         return default;
      }
      
      return JsonConvert.DeserializeObject<T[]>(json.text);
   }
   
}
