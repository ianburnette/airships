using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFogMasking : MonoBehaviour {

    [SerializeField] Texture2D fogTexture;
    [SerializeField] RenderTexture renderTexture;
    [SerializeField] Material mat;
    [SerializeField] Renderer renderer;

    int at = 0;
    [SerializeField] float castDist;
    [SerializeField] LayerMask fogMask;

    public Vector2 hitTexCoords;
    public Vector2Int hitTexCoordsInt;
    public Vector3 hitPoint;

    void OnEnable() {
        fogTexture = new Texture2D (renderTexture.width, renderTexture.height);
        renderer.material.mainTexture = fogTexture;
    }

    void Update() {
        SetupTextureInput();
        var hit = GetHit();
        if (hit.transform == null) {
            WriteAndCloseTextureInput();
            return;
        }

        hitPoint = hit.point;
        hitTexCoords = hit.textureCoord;
        hitTexCoordsInt = new Vector2Int((int)hitTexCoords.x, (int)hitTexCoords.y);
        DebugFogHit(hit);
        ApplyTexture(hitTexCoordsInt);
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
        fogTexture.SetPixel((int)(hitTextureCoord.x * fogTexture.width), (int)(hitTextureCoord.y * fogTexture.height), Color.green);
    }

    void WriteAndCloseTextureInput() {
        fogTexture.Apply();
        RenderTexture.active = null;
    }

    void SetupTextureInput() {
        RenderTexture.active = renderTexture;
        fogTexture.ReadPixels(new Rect(0, 0, fogTexture.width, fogTexture.height), 0, 0);
    }

    RaycastHit GetHit() {
        RaycastHit hit = new RaycastHit();
        Debug.DrawRay(transform.position,transform.forward, Color.magenta);
        return Physics.Raycast(transform.position, Vector3.forward, out hit, castDist, fogMask) ? hit : hit;
    }
}
