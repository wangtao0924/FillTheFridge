using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;

namespace  MPStudio 
{
        public class MPUIImageLocalization : MPUILocalization<Image>
    {
        public enum PathType 
        {
            // 自动通过配置表中的url识别
            Auto = 0,
            // Resources方式加载
            Resources,
            // 网络加载
            Net
        }
        
        [SerializeField]
        protected PathType m_PathType = PathType.Auto;
        
        protected override void RefreshShow()
        {
            string url = GetLocalizationData();
            
            switch (m_PathType)
            {
                case PathType.Auto:
                LoadAuto(url, m_Target);
                break;
                case PathType.Resources:
                StartCoroutine(LoadRes(url, m_Target));
                break;
                case PathType.Net:
                StartCoroutine(LoadNet(url, m_Target));
                break;
            }
        }
        
        private void LoadAuto(string url, Image target)
        {
            if (url.StartsWith("Res://"))
            {
                string targetURL = url.Remove(0, 6);
                StartCoroutine(LoadRes(targetURL, target));
            }
            else if (url.StartsWith("Net://"))
            {
                string targetURL = url.Remove(0, 6);
                StartCoroutine(LoadNet(targetURL, target));
            }
        }
        
        private IEnumerator LoadNet(string url, Image target)
        {
            UnityWebRequest webRequest = new UnityWebRequest(url);
            DownloadHandlerTexture texDl = new DownloadHandlerTexture(true);
            webRequest.downloadHandler = texDl;
            yield return webRequest.SendWebRequest();
            if (webRequest.result != UnityWebRequest.Result.ConnectionError &&
            webRequest.result != UnityWebRequest.Result.ProtocolError)
            {
                Texture2D texture = texDl.texture;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f));
                target.sprite = sprite;
            }
        }
        
        private IEnumerator LoadRes(string url, Image target)
        {
            ResourceRequest request = Resources.LoadAsync<Sprite>(url);
            yield return request;
            Sprite sprite = request.asset as Sprite;
            //Sprite sprite = Resources.Load<Sprite>(url);
            target.sprite = sprite;
        }
    }
}