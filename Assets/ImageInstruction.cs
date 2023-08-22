using UnityEngine;
using UnityEngine.UI;

public class ImageInstruction : MonoBehaviour
{
    public Image image;
    public Image wrongrightImg;
    Sprite sprite;
    public Sprite wrong, right;
    bool isWrong;

    public Sprite Sprite
    {
        get => sprite;
        set
        {
            sprite = value;
            image.sprite = sprite;
        }
    }

    public bool IsWrong
    {
        get => isWrong;
        set
        {
            isWrong = value;
            if (isWrong)
                wrongrightImg.sprite = wrong;
            else
                wrongrightImg.sprite = right;
        }
    }
}
