﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MtgLifeCounter
{
    public class NotifyBase : INotifyPropertyChanged
    {
        public bool Changed { get; set; }

        public NotifyBase()
        {
            Changed = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void FirePropertyChanged<TValue>(Expression<Func<TValue>> propertySelector)
        {
            if (PropertyChanged == null)
                return;

            var memberExpression = propertySelector.Body as MemberExpression;
            if (memberExpression == null)
                return;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberExpression.Member.Name));
        }

        public void Fire(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

    }
    public class PlayerViewModel : NotifyBase
    {
        int _lifeTotal = 20;
        CustomCounterType _counterType = CustomCounterType.Energy;
        int _energy = 0;
        int _experience = 0;
        int _poison = 0;
        string _name = "Player";

        int _cmdEnemy1 = 0;
        int _cmdEnemy2 = 0;
        int _cmdEnemy3 = 0;

        BackGroundColors _color = BackGroundColors.Red;

        PlayerID _id = PlayerID.Unknown;

        Gametypes _type = Gametypes.MultiPlayer;

        public int LifeTotal { get { return _lifeTotal; } set { if (value == _lifeTotal) return; _lifeTotal = value; FirePropertyChanged(() => this.LifeTotal); } }
        public CustomCounterType CounterType { get { return _counterType; } set { if (value == _counterType) return; _counterType = value; FirePropertyChanged(() => this.CounterType); } }
        public int Energy { get { return _energy; } set { if (value == _energy) return; _energy = value; FirePropertyChanged(() => this.Energy); } }
        public int Poison { get { return _poison; } set { if (value == _poison) return; _poison = value; FirePropertyChanged(() => this.Poison); } }
        public int Experience { get { return _experience; } set { if (value == _experience) return; _experience = value; FirePropertyChanged(() => this.Experience); } }
        public string PlayerName { get { return _name; } set { if (value == _name) return; _name = value; FirePropertyChanged(() => this.PlayerName); } }

        public int CmdEnemy1 { get { return _cmdEnemy1; } set { if (value == _cmdEnemy1) return; _cmdEnemy1 = value; FirePropertyChanged(() => this.CmdEnemy1); } }
        public int CmdEnemy2 { get { return _cmdEnemy2; } set { if (value == _cmdEnemy2) return; _cmdEnemy2 = value; FirePropertyChanged(() => this.CmdEnemy2); } }
        public int CmdEnemy3 { get { return _cmdEnemy3; } set { if (value == _cmdEnemy3) return; _cmdEnemy3 = value; FirePropertyChanged(() => this.CmdEnemy3); } }

        public BackGroundColors Color { get { return _color; } set { if (value == _color) return; _color = value; FirePropertyChanged(() => this.Color); } }

        public PlayerID ID { get { return _id; } set { if (value == _id) return; _id = value; FirePropertyChanged(() => this.ID); } }

        public Gametypes GameType { get { return _type; } set { if (value != Gametypes.Current && value == _type) return; _type = value; FirePropertyChanged(() => this.GameType); } }
      
    }
}
