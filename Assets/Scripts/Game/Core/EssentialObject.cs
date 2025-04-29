using System.Linq;
using UnityEngine;

public class EssentialObject : MonoBehaviour 
{
    public int instanceID; //debug for now to show that im keeping the right one
    private void Awake()
    {
        //Grab the original essentail objects each time as the new instantion wouldnt know what has been set in the other
        EssentialObject originalInstance = FindObjectsByType<EssentialObject>(FindObjectsSortMode.InstanceID).First();

        //set first load to instance
        instanceID = GetInstanceID();

        //logic to only load on first scene load
        if (instanceID != originalInstance.instanceID) //if there is another essential objects already in the scene
        {
            Destroy(gameObject); //destroy the new object trying to instantiate
        }

        //dont destroy first load of this object
        DontDestroyOnLoad(gameObject);
    }
}
