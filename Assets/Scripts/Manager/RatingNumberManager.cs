using System.Linq;
using UnityEngine;

public class RatingNumberManager : MonoBehaviour
{
    [Header("Rating Number Item")]
    public RatingNumItemController[] RatingNumItemController;
    public void Load(string rating)
    {
        if (rating.Length > 5)
        {
            rating = rating.Substring(0, 5);
        }

        if (rating.Length == 0)
        {
            RatingNumItemController[4].Load(0);
            for (int i = 3; i >= 0; i--)
            {
                RatingNumItemController[i].Load(-1);
            }
            return;
        }

        var digits = rating.Select(c => c - '0').ToArray();

        var reversedDigits = digits.Reverse().ToArray();

        for (int i = 0; i < 5; i++)
        {
            int value = i < reversedDigits.Length ? reversedDigits[i] : -1;
            RatingNumItemController[4 - i].Load(value);
        }
    }


}
