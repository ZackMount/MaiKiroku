using Assets.Scripts.Models;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Manager
{
    public class PlayerInfoManager : MonoBehaviour
    {
        public TextMeshProUGUI Rating;

        public void Load(Api.Lxns.Models.Player player)
        {
            Rating.text = $"{player.rating}";
        }

    }
}
