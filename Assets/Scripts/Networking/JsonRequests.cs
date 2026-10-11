using System.Text;
using UnityEngine.Networking;

public static class JsonRequests
{
    private const string JsonContentType = "application/json";

    public static UnityWebRequest CreatePost(string url, string json)
    {
        UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", JsonContentType);
        return request;
    }
}
