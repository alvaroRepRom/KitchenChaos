using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "sfxVolume";

    public static SoundManager Instance { get; private set; }


    [SerializeField] private AudioClipRefSO audioClipRefSO;

    private float volume = 1;

    private void Awake()
    {
        Instance = this;

        volume = PlayerPrefs.GetFloat( PLAYER_PREFS_SOUND_EFFECTS_VOLUME , 1f );
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        Player.Instance.OnPickSomething += Instance_OnPickSomething;
        BaseCounter.OnAnyObjectPlaceHere += BaseCounter_OnAnyObjectPlaceHere;
        TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
    }

    private void TrashCounter_OnAnyObjectTrashed( object sender , System.EventArgs e )
    {
        TrashCounter trashCounter = sender as TrashCounter;
        PlaySound( audioClipRefSO.trash , trashCounter.transform.position );
    }


    private void BaseCounter_OnAnyObjectPlaceHere( object sender , System.EventArgs e )
    {
        BaseCounter baseCounter = sender as BaseCounter;
        PlaySound( audioClipRefSO.objectDrop , baseCounter.transform.position );
    }

    private void Instance_OnPickSomething( object sender , System.EventArgs e )
    {
        PlaySound( audioClipRefSO.objectPickUp , Player.Instance.transform.position );
    }

    private void CuttingCounter_OnAnyCut( object sender , System.EventArgs e )
    {
        CuttingCounter cuttingCounter = sender as CuttingCounter;
        PlaySound( audioClipRefSO.chop , cuttingCounter.transform.position );
    }



    private void DeliveryManager_OnRecipeFailed( object sender , System.EventArgs e )
    {
        PlaySound( audioClipRefSO.deliveryFailed , DeliveryCounter.Instance.transform.position );
    }

    private void DeliveryManager_OnRecipeSuccess( object sender , System.EventArgs e )
    {
        PlaySound( audioClipRefSO.deliverySuccess , DeliveryCounter.Instance.transform.position );
    }




    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1)
    {
        PlaySound( audioClipArray[Random.Range( 0 , audioClipArray.Length )] , position , volume );
    }
    
    private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1)
    {
        AudioSource.PlayClipAtPoint( audioClip , position , volumeMultiplier * volume);
    }


    public void PlayFootstepsSound(Vector3 position, float volume)
    {
        PlaySound( audioClipRefSO.footstep , position , volume );
    }


    public void ChangeVolume()
    {
        volume += 0.1f;
        if ( volume > 1 )
            volume = 0;

        PlayerPrefs.SetFloat( PLAYER_PREFS_SOUND_EFFECTS_VOLUME , volume );
        PlayerPrefs.Save();
    }

    public float GetVolume() => volume;
}
