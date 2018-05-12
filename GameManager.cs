using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MtgLifeCounter
{
    public interface IGameManager
    {
        event EventHandler<ColorChangedEvent> PlayerColorChanged;
        int GetNumOfPlayers();
        List<PlayerID> ActivePlayers();
        int GetLifeValue(Gametypes type);

        void ResetPlayers(Gametypes type);
    }


    public class GameManager : IGameManager
    {

        private List<PlayerViewModel> PlayerModels = new List<PlayerViewModel>();

        public event EventHandler<ColorChangedEvent> PlayerColorChanged;

        public PlayerViewModel Player1 { get { return PlayerModels?.ElementAt(0); } }
        public PlayerViewModel Player2 { get { return PlayerModels?.ElementAt(1); } }
        public PlayerViewModel Player3 { get { return PlayerModels?.ElementAt(2); } }
        public PlayerViewModel Player4 { get { return PlayerModels?.ElementAt(3); } }


        public GameManager(int players = 4)
        {
            PlayerModels.Add(new PlayerViewModel() { ID = PlayerID.Player1, PlayerName = "Player 1" });
            PlayerModels.Add(new PlayerViewModel() { ID = PlayerID.Player2, PlayerName = "Player 2" });
            if (players >= 3)
                PlayerModels.Add(new PlayerViewModel() { ID = PlayerID.Player3, PlayerName = "Player 3" });
            if (players >= 4)
                PlayerModels.Add(new PlayerViewModel() { ID = PlayerID.Player4, PlayerName = "Player 4" });

            foreach (var model in PlayerModels)
            {
                model.PropertyChanged += Model_PropertyChanged;
            }

        }


        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            PlayerViewModel model = sender as PlayerViewModel;
            if (model == null)
            {
                foreach (var m in PlayerModels)
                {
                    if (m == sender as PlayerViewModel)
                    {
                        model = m;
                        break;
                    }
                }
            }

            if (model == null)
                return;


            if (e.PropertyName == "Color")
            {
                PlayerColorChanged?.Invoke(this, new ColorChangedEvent(model.ID, model.Color));
            }           
        }

        public int GetNumOfPlayers()
        {
            return PlayerModels.Count;
        }

        public List<PlayerID> ActivePlayers()
        {
            return PlayerModels.Select(p => p.ID).ToList();
        }

        public int GetLifeValue(Gametypes type)
        {
            switch (type)
            {
                default:
                case Gametypes.MultiPlayer:
                    return 20;
                case Gametypes.Commander:
                    return 40;
                case Gametypes.Brawl:
                    return 30;
                 
            }
        }

        public void ResetPlayers(Gametypes type)
        {
            Player1.GameType = type;
            Player2.GameType = type;
            Player3.GameType = type;
            Player4.GameType = type;

        }
    }

    public class ColorChangedEvent : PlayerChangedEvent
    {
        public BackGroundColors Color;

        public ColorChangedEvent(PlayerID id, BackGroundColors color)
            : base(id)
        {
            Color = color;
        }
    }

    public class PlayerChangedEvent : EventArgs
    {
        public PlayerID ID;        

        public PlayerChangedEvent(PlayerID id)
        {
            ID = id;
        }
    }
}
