using UnityEngine;
using UnityEngine.EventSystems;
public class Spinner : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private static Spinner instance;
    public static Spinner Instance => instance;
    private Vector2 scrPos_SpinnerCenter;//质心屏幕坐标
    private void Awake()
    {
        instance = this;
        scrPos_SpinnerCenter = Camera.main.WorldToScreenPoint(transform.position);
    }

    Vector2 vTemp1, vTemp2;
    [SerializeField]
    bool isColckwise, isDraging, isFirstControl;
    float strength, rotZ;
    public void OnBeginDrag(PointerEventData eventData)
    {
        //print("OnBeginDrag");
        vTemp1 = (eventData.position - scrPos_SpinnerCenter).normalized;
        rotZ = transform.rotation.eulerAngles.z;
        isFirstControl = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //print("OnDrag");
        //触摸点到轴心的距离限制
        if (Vector2.Distance(eventData.position, scrPos_SpinnerCenter) < distanceLimit) return;
        vTemp2 = (eventData.position - scrPos_SpinnerCenter).normalized;
        isDraging = false;
        if (SpeedCurrentAbs < speedProtection)
        {
            if (isFirstControl)
            {
                isFirstControl = false;
                vTemp1 = (eventData.position - scrPos_SpinnerCenter).normalized;
                rotZ = transform.rotation.eulerAngles.z;
            }
            isDraging = true;
            var isColckwiseForTouch = IsColckwise();
            isColckwise = IsColckwise(eventData);
            //随手指滑动
            float cosTheta = Vector2.Dot(vTemp1, vTemp2);
            float theta = Mathf.Acos(cosTheta) * Mathf.Rad2Deg * (isColckwiseForTouch ? -1 : 1);
            transform.rotation = Quaternion.Euler(0, 0, rotZ + theta);
            speedCurrent = 0;
        }
    }

    //求方向
    private bool IsColckwise() => vTemp1.x * vTemp2.y - vTemp2.x * vTemp1.y < 0;
    private bool IsColckwise(PointerEventData eventData) => vTemp2.x * eventData.delta.y - eventData.delta.x * vTemp2.y < 0;
    public void OnEndDrag(PointerEventData eventData)
    {
        //print("OnEndDrag");
        isDraging = false;
        var isColckwiseOnEnd = IsColckwise(eventData);
        if (isColckwiseOnEnd == isColckwise) //加速不可逆
        {
            //触摸点到轴心的距离限制
            if (Vector2.Distance(eventData.position, scrPos_SpinnerCenter) < distanceLimit) return;
            //求力度
            strength = eventData.delta.magnitude * strengthScale;
        }
    }

    [SerializeField]
    private float speedCurrent = 0, speedIncrease = 360, speedDecrease = 2;
    public float SpeedCurrentAbs => Mathf.Abs(speedCurrent);
    [SerializeField, Min(0)]
    float speedProtection = 180f, distanceLimit = 50, strengthScale = 1;
    private void Update()
    {
        if (!isDraging) //陀螺跟手时不减速
        {
            //减速
            float speedD = Mathf.Lerp(1, speedDecrease, SpeedCurrentAbs / 3160f);//优化阻力
            var anti = speedD * (isColckwise ? -1 : 1);
            var speedCurrentTemp = speedCurrent;
            speedCurrent += anti;
            if (speedCurrentTemp * speedCurrent <= 0) speedCurrent = 0;
        }
        //加速
        speedCurrent += (speedIncrease * strength) * (isColckwise ? 1 : -1);
        strength = 0;
    }

    private void LateUpdate()
    {
        transform.Rotate(Vector3.back * speedCurrent * Time.deltaTime);
    }

}
