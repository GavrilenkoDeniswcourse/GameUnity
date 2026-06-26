using UnityEngine;
using UnityEngine.Rendering;

public class NewEmptyCSharpScript : MonoBehaviour
{
    private void Awake()
    {
        GraphicsSettings.transparencySortMode = TransparencySortMode.CustomAxis;
        GraphicsSettings.transparencySortAxis = new Vector3(0.0f,1.0f,0.0f);

        Debug.Log("Good");
    }


}
