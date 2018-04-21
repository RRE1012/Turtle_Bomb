using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoyStickMove : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public static JoyStickMove instance;
    Vector3 inputVector;
    public RawImage joystick_HandleImage;
    private RawImage joystick_BackGroundImage;
    private Vector3 unNormalizedInput;
    public int joystickHandleDistance = 2;
    private Vector3[] fourCornersArray = new Vector3[4]; // used to get the bottom right corner of the image in order to ensure that the pivot of the joystick's background image is always at the bottom right corner of the image (the pivot must always be placed on the bottom right corner of the joystick's background image in order to the script to work)
    private Vector2 bgImageStartPosition;
    bool touched = false;

    private void Awake()
    {
        instance = this;
    }
    // Use this for initialization
    void Start () {
        joystick_BackGroundImage = GetComponent<RawImage>();
        joystick_BackGroundImage.rectTransform.GetWorldCorners(fourCornersArray); 

        bgImageStartPosition = fourCornersArray[3]; 
        joystick_BackGroundImage.rectTransform.pivot = new Vector2(1, 0); 

        joystick_BackGroundImage.rectTransform.anchorMin = new Vector2(0, 0); 
        joystick_BackGroundImage.rectTransform.anchorMax = new Vector2(0, 0); 
        joystick_BackGroundImage.rectTransform.position = bgImageStartPosition;
    }
	public float GetJoyPosX()
    {
        return joystick_HandleImage.rectTransform.anchoredPosition.x;
    }
    public float GetJoyPosZ()
    {
        return joystick_HandleImage.rectTransform.anchoredPosition.y;
    }
    // Update is called once per frame
    public virtual void OnDrag (PointerEventData ped) {
        Vector2 localP = Vector2.zero;
       
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystick_BackGroundImage.rectTransform, ped.position, ped.pressEventCamera, out localP))
        {
            localP.x = (localP.x / joystick_BackGroundImage.rectTransform.sizeDelta.x);
            localP.y = (localP.y / joystick_BackGroundImage.rectTransform.sizeDelta.y);

            inputVector = new Vector3(localP.x * 2 + 1, localP.y * 2 - 1, 0);
            unNormalizedInput = inputVector;
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

            joystick_HandleImage.rectTransform.anchoredPosition = new Vector2(inputVector.x * (joystick_BackGroundImage.rectTransform.sizeDelta.x / joystickHandleDistance),
                         inputVector.y * (joystick_BackGroundImage.rectTransform.sizeDelta.y / joystickHandleDistance));


        }
    }
    /*
     Debug 용
    private void Update()
    {
        Debug.Log(joystick_HandleImage.rectTransform.anchoredPosition);
    }
    */
    public virtual void OnPointerDown(PointerEventData eventP)
    {
        Debug.Log("True");
        touched = true;
        OnDrag(eventP);
    }
    public virtual void OnPointerUp(PointerEventData eventP)
    {
        inputVector = Vector3.zero; 
        joystick_HandleImage.rectTransform.anchoredPosition = new Vector2(0,0); 
    }
    public void Touched()
    {
        touched = true;
    }
    public void UnTouched()
    {
        touched = false;
    }
}
