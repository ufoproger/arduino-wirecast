/*
 Wirecast 4.3 .NET Wrapper v 0.1 by Kristijan Burnik <kristijanburnik@gmail.com>
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace WirecastWrapper
{
    public class WirecastShot
    {
        public int ShotID { get; private set; }
        public string Name { get; private set; }

        private static Dictionary<int, WirecastShot> shots = new Dictionary<int, WirecastShot>();

        private WirecastShot() { }

        public static WirecastShot CreateFromObject(int shotID, object shot)
        {
            string name = (string)Late.Get(shot, "Name");
			// Не нужно кеширование имени текущего кадра, вдруг название кадра изменится!
			return new WirecastShot() { ShotID = shotID, Name = name };

            if (!shots.ContainsKey(shotID))
            {
                shots[shotID] = new WirecastShot() { ShotID = shotID, Name = name };
            }

            return shots[shotID];

        }

		public bool Equals(WirecastShot b)
		{
			return (this.Name == b.Name && this.ShotID == b.ShotID);
		}

		override public string ToString()
		{
			return "\"" + this.Name + "\" (" + this.ShotID + ")";
		}
    }

}
