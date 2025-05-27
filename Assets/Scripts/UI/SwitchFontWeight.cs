using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SwitchFontWeight : MonoBehaviour
{
    [SerializeField] Button button1,button2,button3;
    [SerializeField] Color buttonActiveColor;
    [SerializeField] Color buttonDefaultColor;

    public void Button_Selected_2(string btnName)
    {
        if(btnName == "bold") 
        {
            button1.image.color = buttonActiveColor;
            button2.image.color = buttonDefaultColor;
        } else 
        {
            button2.image.color = buttonActiveColor;
            button1.image.color = buttonDefaultColor;
        }
    }
    public void Button_Selected_3(string btnName)
    {
        if(btnName == "div1") 
        {
            button1.image.color = buttonActiveColor;
            button2.image.color = buttonDefaultColor;
            button3.image.color = buttonDefaultColor;
        } else if(btnName == "div2") 
        {
            button1.image.color = buttonDefaultColor;
            button2.image.color = buttonActiveColor;
            button3.image.color = buttonDefaultColor;
        } else 
        {
            button1.image.color = buttonDefaultColor;
            button2.image.color = buttonDefaultColor;
            button3.image.color = buttonActiveColor;
        }   
    }

}
