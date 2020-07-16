using UnityEngine;

public class ARManager : MonoBehaviour
{
    [SerializeField] GameObject animationPrefab;
    [SerializeField] GameObject adPrefab;
    [SerializeField] Camera arCamera;
    private GameObject _jukebox;
    private GameObject _animationCube;
    private AudioSource _audioSource;

    private float destroyTime = 60.0f;
    private float timeElapsed = 0.0f;
    private Vector3 positionJukebox;
    private Vector3 positionCube;
    private Quaternion rotation = Quaternion.Euler(-90f, 0f, 0f);


    void Start()
    {
        //一定時間経つと広告は消去され新たな広告が同時に生成される
        InvokeRepeating(nameof(CreateObject), 0.0f, destroyTime);
    }

    void Update()
    {
        // 広告生成時からの時間を保持
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= destroyTime) timeElapsed = 0.0f;
        // inputTouchが行われてから実行を開始
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase != TouchPhase.Began)
            {
                // RaycastingによってObjectとのCollideを検知
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hitObject;
                if (Physics.Raycast(ray, out hitObject))
                {
                    // ヒットしたcolliderと生成広告と同じであれば音源の再生とAnimationの開始
                    if (_jukebox.name == hitObject.collider.gameObject.name)
                    {
                        // タッチしたらもう一度最初から再生できる様にする
                        _audioSource = _jukebox.GetComponent<AudioSource>();
                        _audioSource.PlayOneShot(_audioSource.clip);
                        PlayAnimationCube();
                    }
                }
            }
        }
    }
    
    void CreateObject()
    {
        var cameraAngle = arCamera.transform.rotation * Vector3.forward;
        // 人の身長を加味して1mほど下にobjectを生成する様にした
        // 生成のタイミングでObjectがカメラの方向に向く様にした
        positionJukebox =
            new Vector3((float) (arCamera.transform.position.x - 1.0 * cameraAngle.x),
                (float) (arCamera.transform.position.y - 1.0 * cameraAngle.y-1.0),
                (float) (arCamera.transform.position.z - 1.0 * cameraAngle.z));
        rotation = Quaternion.Euler(-90f, 0f, arCamera.transform.rotation.eulerAngles.y);
        _jukebox = Instantiate(adPrefab, positionJukebox, rotation);
        Destroy(_jukebox, destroyTime);
    }

    void PlayAnimationCube()
    {
        positionCube =
            new Vector3((float) (positionJukebox.x),
                (float) (positionJukebox.y + 0.8),
                (float) (positionJukebox.z));
        // 一度のみの処理にするためにnullでない時は再生しない様にする．
        if (_animationCube == null)
        {
            _animationCube = Instantiate(animationPrefab, positionCube, rotation);
            Destroy(_animationCube, destroyTime - timeElapsed);
        }
    }
}
