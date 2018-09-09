using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerFogMasking : MonoBehaviour {

    [SerializeField] Texture2D fogTexture;

    [SerializeField] RenderTexture renderTexture;
    [SerializeField] Material mat;
    [SerializeField] Renderer renderer;

    [SerializeField] float castDist;
    [SerializeField] LayerMask fogMask;
    [SerializeField] Color fogClearColor;

    [FormerlySerializedAs("fogClearingTexture")] [SerializeField] Texture2D fogBrush;

    public Vector2 hitTexCoords;
    [SerializeField] Color[] brush1dimensional;

    void OnEnable() {
        //fogTexture = new Texture2D (renderTexture.width, renderTexture.height);
        //fogTexture.width = renderTexture.width;
        //fogTexture.height = renderTexture.height;

        fogTexture.filterMode = FilterMode.Point;
        renderer.material.mainTexture = fogTexture;
        SetBrush();
    }

    void SetBrush() {
        brush1dimensional = new Color[fogBrush.width * fogBrush.height];
        for (var x = 0; x < fogBrush.width; x++) {
            for (var y = 0; y < fogBrush.height; y++) {
                brush1dimensional[x * y] = fogBrush.GetPixel(x, y);
            }
        }
    }

    void Update() {
        SetupTextureInput();
        var hit = GetHit();
        if (hit.transform == null) {
            WriteAndCloseTextureInput();
            return;
        }
        hitTexCoords = hit.textureCoord;
        DebugFogHit(hit);
        ApplyTexture(hitTexCoords);
        WriteAndCloseTextureInput();
        //for (var i =0; i<renderTexture.width*.2f;i++)
        //    for (var j =0;j<renderTexture.height;j++)
        //        fogTexture.SetPixel((at +i),j,new Color(1,0,0));
//
        /*
        Graphics.SetRenderTarget(renderTexture);
        RenderTexture.active = renderTexture;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, 512, 512, 0);
        Graphics.DrawTexture(new Rect(transform.position.x, transform.position.y, 10, 10), renderTexture);
        GL.PopMatrix();
        RenderTexture.active = null;
        Graphics.SetRenderTarget(null);*/
    }

    static void DebugFogHit(RaycastHit hit) {
        Debug.DrawRay(hit.point, Vector2.left / 3, Color.green);
        Debug.DrawRay(hit.point, Vector2.right / 3, Color.green);
        Debug.DrawRay(hit.point, Vector2.up / 3, Color.green);
        Debug.DrawRay(hit.point, Vector2.down / 3, Color.green);
    }

    void ApplyTexture(Vector2 hitTextureCoord) {
       // var x = (int)hitTextureCoord.x * fogTexture.width;
       // var y = (int)hitTextureCoord.y * fogTexture.height;

        //fogTexture.SetPixel
        //    ((int)(hitTextureCoord.x * fogTexture.width),
        //     (int)(hitTextureCoord.y * fogTexture.height),
        //     fogClearColor);
////
        var cols = new Color[1] { fogClearColor };
        fogTexture.SetPixels((int)(hitTextureCoord.x * fogTexture.width),
                             (int)(hitTextureCoord.y * fogTexture.height),
                             fogBrush.width, fogBrush.height, brush1dimensional);
        //
        //fogTexture.SetPixel(x, y, fogClearColor);
        //
        //fogBrush.width, fogBrush.height,
        //brush1dimensional);
    }

    void WriteAndCloseTextureInput() {
        fogTexture.Apply();
        RenderTexture.active = null;
    }

    void SetupTextureInput() {
        RenderTexture.active = renderTexture;
        //fogTexture.ReadPixels(new Rect(0, 0, fogTexture.width, fogTexture.height), 0, 0);
    }

    RaycastHit GetHit() {
        RaycastHit hit;
        Debug.DrawRay(transform.position,transform.forward, Color.magenta);
        Physics.Raycast(transform.position, Vector3.forward, out hit, castDist, fogMask);
        return hit;
    }
}
